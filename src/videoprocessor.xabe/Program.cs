using Azure.Storage.Blobs;
using Azure.Storage;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text.Json;
using Xabe.FFmpeg;
using System.Resources;
using System;

namespace videoprocessor.xabe;

class Program
{
    const string SourceStorageName = "cheuw001assetsstcool";
    const string SourceStorageKey = "";

    const string QueueStorageConnection = "";
    const string QueueName = "demovideoqueue";

    const string OutputFolderName = "generated";

    static async Task Main(string[] args)
    {
        string? mediaPath = Environment.GetEnvironmentVariable("MEDIA_PATH");

        //"C:\\temp\\videos";

        if (mediaPath is not null)
        {
            //QueueClient queueClient = new(QueueStorageConnection, QueueName);

            //QueueMessage message = await queueClient.ReceiveMessageAsync(new TimeSpan(0, 0, 5)); // Max four hours to process

            //Console.WriteLine($"Received message{message.Body}");

            //JsonSerializerOptions serializeOptions = new JsonSerializerOptions()
            //{
            //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            //};

            //AssetMessage assetMessage = JsonSerializer.Deserialize<AssetMessage>(message.Body.ToString(), serializeOptions)
            //    ?? new()
            //    {
            //        AssetContianerName = string.Empty,
            //        AssetId = string.Empty,
            //        OriginalAssetBlobName = string.Empty
            //    };

            AssetMessage assetMessage = new()
            {
                AssetContianerName = "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                AssetId = "a3a45e98-a2ce-4b37-84ab-3fbd1cae624f",
                OriginalAssetBlobName = "original",
                OutFilePrefix = "friendseating_"
            };

            Console.WriteLine($"Asset Id: {assetMessage.AssetId}");
            Console.WriteLine($"Asset Container Name: {assetMessage.AssetContianerName}");

            string assetFolderPath = Path.Combine(mediaPath, assetMessage.AssetId);
            string assetPath = await DownloadOrginalAsset(assetMessage.AssetContianerName,
                assetFolderPath,
                assetMessage.AssetId,
                assetMessage.OriginalAssetBlobName);

            Console.WriteLine($"Media path is: {mediaPath}");
            Console.WriteLine($"Asset path is: {assetPath}");

            string outputFolderPath = Path.Combine(assetFolderPath, OutputFolderName);

            Directory.CreateDirectory(outputFolderPath);

            IMediaInfo info = await FFmpeg.GetMediaInfo(assetPath);
            Console.WriteLine($"Asset duration is: {info.Duration}");

            IStream? videoStream = info.VideoStreams.FirstOrDefault()?
                .SetSize(VideoSize.Hd720)
                .SetCodec(VideoCodec.libx264);

            IStream? audioStream = info.AudioStreams.FirstOrDefault();

            IConversionResult resultVideo = await FFmpeg.Conversions.New()
                .AddStream(videoStream, audioStream)
                .SetOutput(Path.Combine(outputFolderPath, string.Concat(assetMessage.OutFilePrefix, "720.mp4")))
                .Start();

            Func<string, string> outputFileNameBuilder = (number) => { return Path.Combine(outputFolderPath, string.Concat(assetMessage.OutFilePrefix, number, ".png")); };

            IVideoStream? videoStreamForImages = info.VideoStreams.First()?.SetCodec(VideoCodec.png);
            int frameExctractionRate = (int) (videoStreamForImages.Framerate * videoStreamForImages.Duration.Seconds) / 10;

            Console.WriteLine($"Frame extraction rate is:{frameExctractionRate}");

            IConversionResult resultImages = await FFmpeg.Conversions.New()
                .AddStream(videoStreamForImages)
                .ExtractEveryNthFrame(frameExctractionRate, outputFileNameBuilder)
                .Start();

            Console.WriteLine($"Video covertion duration is: {resultVideo.Duration}");
            Console.WriteLine($"Image covertion duration is: {resultImages.Duration}");
            Console.WriteLine($"Process duration is: {resultImages.Duration.Add(resultVideo.Duration)}");
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
