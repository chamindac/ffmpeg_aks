using Azure.Storage.Blobs;
using Azure.Storage;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text.Json;
using Xabe.FFmpeg;
using System.Resources;
using System;
using Azure;
using System.Diagnostics;
using System.ComponentModel;

namespace videoprocessor.xabe;

class Program
{
    const string SourceStorageName = "cheuw001assetsstcool";
    const string SourceStorageKey = "";

    const string destinationStorageName = "cheuw001assetssthot";
    const string destinationStorageKey = "";

    const string QueueStorageConnection = "";
    const string QueueName = "dotnetvideoqueue";

    const string OutputFolderName = "generated";

    static async Task Main(string[] args)
    {
        // Use mounted volume path for container
        string? mediaPath = Environment.GetEnvironmentVariable("MEDIA_PATH");

        if (mediaPath is not null)
        {
            // Get first message from queue with 4 hour hide time for message and exist if no messages
            QueueClient queueClient = new(QueueStorageConnection, QueueName);
            QueueMessage message = await queueClient.ReceiveMessageAsync(new TimeSpan(0, 4, 0)); // Max four hours to process

            if (message is null)
            {
                Console.WriteLine($"No messages to process");
                return;
            }   

            Console.WriteLine($"Received message{message.Body}");

            // Extract received message body
            JsonSerializerOptions serializeOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            AssetMessage assetMessage = JsonSerializer.Deserialize<AssetMessage>(message.Body.ToString(), serializeOptions)
                ?? new()
                {
                    AssetContianerName = string.Empty,
                    AssetId = string.Empty,
                    OriginalAssetBlobName = string.Empty,
                    OutFilePrefix = string.Empty
                };

            // Create video download folder
            Console.WriteLine($"Video Asset Id: {assetMessage.AssetId}");
            Console.WriteLine($"Video Asset Container Name: {assetMessage.AssetContianerName}");
            string assetFolderPath = Path.Combine(mediaPath, assetMessage.AssetId);

            try
            {
                // Download original video file
                Stopwatch downloadTimer = Stopwatch.StartNew();
                string assetPath = await DownloadOrginalAsset(assetMessage.AssetContianerName,
                    assetFolderPath,
                    assetMessage.AssetId,
                    assetMessage.OriginalAssetBlobName);

                downloadTimer.Stop();
                Console.WriteLine($"Media path is: {mediaPath}");
                Console.WriteLine($"Video asset path is: {assetPath}");

                // Obtain video file size in MBs and duration
                FileInfo assetInfo = new FileInfo(assetPath);
                long assetSize = assetInfo.Length / (1024 * 1024);
                string outputFolderPath = Path.Combine(assetFolderPath, OutputFolderName);
                Console.WriteLine($"Video file size is: {assetSize} MBs");

                IMediaInfo info = await FFmpeg.GetMediaInfo(assetPath);
                Console.WriteLine($"Video duration is: {info.Duration}");

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
                    .SetOutput(Path.Combine(outputFolderPath, string.Concat(assetMessage.OutFilePrefix, "_720.mp4")))
                    .AddParameter("-crf 28 -preset ultrafast -c:a copy")
                    .Start();

                // Delegate for image name
                Func<string, string> outputFileNameBuilder = (number) => { return Path.Combine(outputFolderPath, string.Concat(assetMessage.OutFilePrefix, number, ".png")); };

                // Obtain video file for extacting images as png
                IVideoStream videoStreamForImages = info.VideoStreams.First().SetCodec(VideoCodec.png);
                // Calculate image extraction frame rate, to extract 10 images from the video
                int frameExctractionRate = (int)(videoStreamForImages.Framerate * videoStreamForImages.Duration.TotalSeconds) / 9; // Divide by 10 gives 11 images

                Console.WriteLine($"Frame extraction rate is:{frameExctractionRate}");

                // Extract images
                IConversionResult resultImages = await FFmpeg.Conversions.New()
                    .AddStream(videoStreamForImages)
                    .ExtractEveryNthFrame(frameExctractionRate, outputFileNameBuilder)
                    .Start();

                Console.WriteLine($"Video covertion duration is: {resultVideo.Duration}");
                Console.WriteLine($"Image covertion duration is: {resultImages.Duration}");
                Console.WriteLine($"Process duration is: {resultImages.Duration.Add(resultVideo.Duration)}");

                // Upload generated 720p video and exxtracted images to target blob storage
                Stopwatch uploadTimer = Stopwatch.StartNew();
                BlobServiceClient destinationBlobServiceClient = GetBlobServiceClient(destinationStorageName,
                    destinationStorageKey);

                BlobContainerClient destinationContainer = await CreateDestinationContainer(destinationBlobServiceClient,
                    string.Concat("dotnet-", assetMessage.AssetId));

                await UploadGeneratedAssets(destinationContainer, outputFolderPath);
                uploadTimer.Stop();
                Console.WriteLine($"File upload duration is:{uploadTimer.Elapsed.TotalSeconds} seconds");

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
                await blob.UploadAsync(processTimeInfoFilePath, overwrite:true);

                // Remove processed message from queue
                await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                Console.WriteLine($"Successfully processed in {message.DequeueCount} attempt(s). Removing message from the queue...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                // Allow retry message processing for 3 attempts, remove message if 3 attempts and still failing
                if (message.DequeueCount >= 3)
                {
                    Console.WriteLine("Failed to process in 3 attempts. Removing message from the queue...");
                    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                    Console.WriteLine("Message removed from the queue.");
                }
                else
                {
                    Console.WriteLine($"Failed to process in {message.DequeueCount} attempt. Adding message back to the queue...");
                    await queueClient.UpdateMessageAsync(message.MessageId, message.PopReceipt, visibilityTimeout: new TimeSpan(0, 0, 1));
                    Console.WriteLine("Message added back to the queue.");
                }
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
    }

    private static async Task<BlobContainerClient> CreateDestinationContainer(BlobServiceClient blobServiceClient, string containerName)
    {
        BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync();
        return container;
    }

    private static async Task UploadGeneratedAssets(BlobContainerClient container, string generatedAssetsPath)
    {


        try
        {
            Console.WriteLine($"Found {Directory.GetFiles(generatedAssetsPath).Length} file(s)");

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
                Console.WriteLine($"Uploading {fileName} to container {container.Name}");
                BlobClient blob = container.GetBlobClient(fileName);

                // Add the upload task to the queue
                tasks.Enqueue(blob.UploadAsync(filePath, options));
            }

            // Run all the tasks asynchronously.
            await Task.WhenAll(tasks);
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine($"Azure request failed: {ex.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine($"Error parsing files in the directory: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }

    private static BlobServiceClient GetBlobServiceClient(string accountName, string accountKey)
    {
        Azure.Storage.StorageSharedKeyCredential sharedKeyCredential =
            new StorageSharedKeyCredential(accountName, accountKey);

        string blobUri = "https://" + accountName + ".blob.core.windows.net";

        return new
            (new Uri(blobUri), sharedKeyCredential);
    }

    private static async Task<string> DownloadOrginalAsset(string assetCotainer,
        string assetFolderPath,
        string assetId,
        string originalAssetBlobName)
    {

        BlobServiceClient blobServiceClient = GetBlobServiceClient(SourceStorageName, SourceStorageKey);
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
