﻿using Azure.Identity;
using Azure;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using videoprocessor.eventhub.Interfaces;
using videoprocessor.eventhub.Models;
using System.Diagnostics;
using Xabe.FFmpeg;

namespace videoprocessor.eventhub.Services
{
    internal class VideoTranscoder : IVideoTranscoder
    {
        private const string _sourceStorageName = "cheuw001assetsstcool";
        private const string _destinationStorageName = "cheuw001assetssthot";
        private const string _outputFolderName = "generated";

        private readonly ILogger<VideoTranscoder> _logger;
        private readonly string _trascoderInstanceMediaPath;

        public VideoTranscoder(ILogger<VideoTranscoder> logger)
        {
            _logger = logger;
            _trascoderInstanceMediaPath = Path.Combine(Environment.GetEnvironmentVariable("MEDIA_PATH"), Guid.NewGuid().ToString());
        }
        public void Dispose()
        {
            if (Directory.Exists(_trascoderInstanceMediaPath))
            {
                Directory.Delete(_trascoderInstanceMediaPath, true);
            }
        }

        public async Task TranscodeAsync(TranscodeRequest transcodeRequest)
        {
            _logger.LogInformation($"Transcoding {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}... Media path is: {_trascoderInstanceMediaPath}");

            //// TODO: This is fake implemtation to test scaling kindly ignore for now
            //int elapsedSeconds = 0;
            //while (elapsedSeconds < 60) // TODO: fix here
            //{
            //    await Task.Delay(TimeSpan.FromSeconds(10));
            //    elapsedSeconds += 10;
            //    _logger.LogInformation($"Still transcoding {elapsedSeconds}... {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}...");
            //}
            //_logger.LogInformation($"Transcoding completed for {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}.");

            _logger.LogInformation($"Transcoding {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}...");
            // Create video download folder
            _logger.LogInformation($"Video Asset Id: {transcodeRequest.AssetId}");
            _logger.LogInformation($"Video Asset Container Name: {transcodeRequest.AssetContainerName}");
            string assetFolderPath = Path.Combine(_trascoderInstanceMediaPath, transcodeRequest.AssetId);

            try
            {
                // Download original video file
                Stopwatch downloadTimer = Stopwatch.StartNew();
                string assetPath = await DownloadOrginalAsset(transcodeRequest.AssetContainerName,
                    assetFolderPath,
                    transcodeRequest.AssetId,
                    transcodeRequest.OriginalAssetBlobName);

                downloadTimer.Stop();
                _logger.LogInformation($"Transcoding {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}... Video asset path is: {assetPath}");

                // Obtain video file size in MBs and duration
                FileInfo assetInfo = new FileInfo(assetPath);
                long assetSize = assetInfo.Length / (1024 * 1024);
                string outputFolderPath = Path.Combine(assetFolderPath, _outputFolderName);
                _logger.LogInformation($"Transcoding {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}... Video file size is: {assetSize} MBs");

                IMediaInfo info = await FFmpeg.GetMediaInfo(assetPath);
                _logger.LogInformation($"Transcoding {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}... Video duration is: {info.Duration}");

                // Create output folder to keep generated files for uploading to target
                Directory.CreateDirectory(outputFolderPath);

                // Obtain vidoe stream and set 720p preview size, based on horizontal or vertical video
                IVideoStream? videoStream = info.VideoStreams.FirstOrDefault();
                if (videoStream?.Height > videoStream?.Width)
                {
                    // Vertical
                    videoStream
                        .SetSize(720, 1280)
                        .SetCodec(VideoCodec.libx264);
                }
                else
                {
                    // Hortizontal
                    videoStream?
                        .SetSize(VideoSize.Hd720)
                        .SetCodec(VideoCodec.libx264);
                }

                // Obtain audio stream
                IStream? audioStream = info.AudioStreams.FirstOrDefault();

                // Generate 720p video
                IConversionResult resultVideo = await FFmpeg.Conversions.New()
                    .AddStream(videoStream, audioStream)
                    .SetOutput(Path.Combine(outputFolderPath, string.Concat(transcodeRequest.OutFilePrefix, "_720.mp4")))
                    .AddParameter("-crf 28 -preset ultrafast -c:a copy")
                    .Start();

                // Delegate for image name
                Func<string, string> outputFileNameBuilder = (number) => { return Path.Combine(outputFolderPath, string.Concat(transcodeRequest.OutFilePrefix, number, ".png")); };

                // Obtain video file for extacting images as png
                IVideoStream videoStreamForImages = info.VideoStreams.First().SetCodec(VideoCodec.png);
                // Calculate image extraction frame rate, to extract 10 images from the video
                int frameExctractionRate = (int)(videoStreamForImages.Framerate * videoStreamForImages.Duration.TotalSeconds) / 9; // Divide by 10 gives 11 images

                _logger.LogInformation($"Transcoding {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}... Frame extraction rate is:{frameExctractionRate}");

                // Extract images
                IConversionResult resultImages = await FFmpeg.Conversions.New()
                    .AddStream(videoStreamForImages)
                    .ExtractEveryNthFrame(frameExctractionRate, outputFileNameBuilder)
                    .Start();

                _logger.LogInformation($"Transcoding {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}... Video covertion duration is: {resultVideo.Duration}");
                _logger.LogInformation($"Transcoding {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}... Image covertion duration is: {resultImages.Duration}");
                _logger.LogInformation($"Transcoding {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}... Process duration is: {resultImages.Duration.Add(resultVideo.Duration)}");

                // Upload generated 720p video and exxtracted images to target blob storage
                Stopwatch uploadTimer = Stopwatch.StartNew();
                BlobServiceClient destinationBlobServiceClient = GetBlobServiceClient(_destinationStorageName);

                BlobContainerClient destinationContainer = await CreateDestinationContainer(destinationBlobServiceClient,
                    string.Concat("dotnet-", transcodeRequest.AssetId));

                await UploadGeneratedAssets(destinationContainer, outputFolderPath);
                uploadTimer.Stop();
                _logger.LogInformation($"Transcoding {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}... File upload duration is:{uploadTimer.Elapsed.TotalSeconds} seconds");

                // Generate process time info file
                string processTimeInfoFileName = "processtime.txt";
                string processTimeInfoFilePath = Path.Combine(outputFolderPath, processTimeInfoFileName);
                StreamWriter processTimeInfoFileStream = File.CreateText(processTimeInfoFilePath);
                await processTimeInfoFileStream.WriteLineAsync($"Asset size:{assetSize} MB");
                await processTimeInfoFileStream.WriteLineAsync($"Download duration is:{downloadTimer.Elapsed.TotalSeconds} seconds");
                await processTimeInfoFileStream.WriteLineAsync($"Process duration is:{resultImages.Duration.Add(resultVideo.Duration)}");
                await processTimeInfoFileStream.WriteLineAsync($"Upload duration is:{uploadTimer.Elapsed.TotalSeconds} seconds");
                processTimeInfoFileStream.Close();

                // Upload process time info file
                BlobClient blob = destinationContainer.GetBlobClient(processTimeInfoFileName);
                await blob.UploadAsync(processTimeInfoFilePath, overwrite: true);

                _logger.LogInformation($"Transcoding completed for {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}.");

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Transcoding failed {transcodeRequest.AssetId} as {transcodeRequest.OutFilePrefix}...");
                _logger.LogInformation(ex.Message);
            }
            finally
            {
                // Clean up downloaded and generated files
                if (Directory.Exists(assetFolderPath))
                {
                    Directory.Delete(assetFolderPath, true);
                }
            }
        }

        private static async Task<BlobContainerClient> CreateDestinationContainer(BlobServiceClient blobServiceClient, string containerName)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }

        private async Task UploadGeneratedAssets(BlobContainerClient container, string generatedAssetsPath)
        {
            try
            {
                _logger.LogInformation($"Found {Directory.GetFiles(generatedAssetsPath).Length} file(s) in {generatedAssetsPath}.");

                // Specify the StorageTransferOptions
                BlobUploadOptions options = new BlobUploadOptions
                {
                    TransferOptions = new StorageTransferOptions
                    {
                        // Set the maximum number of workers that 
                        // may be used in a parallel transfer.
                        MaximumConcurrency = 8,

                        // Set the maximum length of a transfer to 50MB.
                        MaximumTransferSize = 50 * 1024 * 1024
                    }
                };

                // Create a queue of tasks that will each upload one file.
                var tasks = new Queue<Task<Response<BlobContentInfo>>>();

                // Iterate through the files
                foreach (string filePath in Directory.GetFiles(generatedAssetsPath))
                {
                    string fileName = Path.GetFileName(filePath);
                    _logger.LogInformation($"Uploading {fileName} to container {container.Name}");
                    BlobClient blob = container.GetBlobClient(fileName);

                    // Add the upload task to the queue. UploadAsync overwrites existing blobs
                    tasks.Enqueue(blob.UploadAsync(filePath, options));
                }

                // Run all the tasks asynchronously.
                await Task.WhenAll(tasks);
            }
            catch (RequestFailedException ex)
            {
                _logger.LogInformation($"Azure request failed: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.LogInformation($"Error parsing files in the directory: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Exception: {ex.Message}");
            }
        }

        private static BlobServiceClient GetBlobServiceClient(string accountName)
        {
            return new(new Uri($"https://{accountName}.blob.core.windows.net"),
                new DefaultAzureCredential());
        }

        private static async Task<string> DownloadOrginalAsset(string assetCotainer,
            string assetFolderPath,
            string assetId,
            string originalAssetBlobName)
        {

            BlobServiceClient blobServiceClient = GetBlobServiceClient(_sourceStorageName);
            BlobClient blobClient = blobServiceClient
                    .GetBlobContainerClient(assetCotainer)
                    .GetBlobClient($"{assetId}/{originalAssetBlobName}");

            if (Directory.Exists(assetFolderPath))
            {
                Directory.Delete(assetFolderPath, true);
            }

            Directory.CreateDirectory(assetFolderPath);

            string localFilePath = Path.Combine(assetFolderPath, assetId);
            FileStream fileStream = File.OpenWrite(localFilePath);

            var transferOptions = new StorageTransferOptions
            {
                // Set the maximum number of parallel transfer workers
                MaximumConcurrency = 5,

                // Set the initial transfer length to 8 MiB
                InitialTransferSize = 8 * 1024 * 1024,

                // Set the maximum length of a transfer to 4 MiB
                MaximumTransferSize = 4 * 1024 * 1024
            };

            BlobDownloadToOptions downloadOptions = new BlobDownloadToOptions()
            {
                TransferOptions = transferOptions
            };

            await blobClient.DownloadToAsync(fileStream, downloadOptions);

            fileStream.Close();

            return localFilePath;
        }
    }
}