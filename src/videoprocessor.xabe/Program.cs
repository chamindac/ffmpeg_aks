using Azure.Storage.Blobs;
using Azure.Storage;
using Azure.Storage.Blobs.Models;

namespace videoprocessor.xabe;

class Program
{
    const string SourceStorageName = "cheuw001assetsstcool";
    const string SourceStorageKey = "shouldbesecretinAKS";
    const string OriginalAssetBlobName = "original";

    static void Main(string[] args)
    {
        string? mediaPath = Environment.GetEnvironmentVariable("MEDIA_PATH");

        if (mediaPath is not null)
        {
            string assetId = "bb5ab2dd-f89c-4689-976b-0de2fce614ec";
            string assetContianer = "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3";
            string assetFolderPath = Path.Combine(mediaPath, assetId);

            string assetPath = DownloadOrginalAsset(assetContianer, assetFolderPath, assetId);

            string[] assetFiles = Directory.GetFiles(assetFolderPath);


            Console.WriteLine($"Media path is: {mediaPath}");
            foreach (var assetFile in assetFiles)
            {
                Console.WriteLine($": {assetFile}");
            }
            
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

    public static string DownloadOrginalAsset(string assetCotainer,
        string assetFolderPath,
        string assetId)
    {

        BlobServiceClient blobServiceClient = GetBlobServiceClient(SourceStorageName, SourceStorageKey);
        BlobClient blobClient = blobServiceClient
                .GetBlobContainerClient(assetCotainer)
                .GetBlobClient($"{assetId}/{OriginalAssetBlobName}");

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

        blobClient.DownloadTo(fileStream, downloadOptions);

        fileStream.Close();

        return localFilePath;
    }
}
