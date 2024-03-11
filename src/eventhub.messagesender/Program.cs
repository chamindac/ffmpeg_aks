using common.lib.Configs;
using Azure.Identity;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Azure.Messaging.EventHubs;
using System.Text;

namespace eventhub.messagesender;

internal class Program
{
    private const string EventHubName = "videopreview";

    static async Task Main(string[] args)
    {
        IConfigurationRoot config = ConfigLoader.LoadConfiguration(new ConfigurationBuilder());

        string eventhubNamespace = config.GetSection("EventHubNamespaceName-1").Value;


        EventHubProducerClient producerClient = new EventHubProducerClient(
            $"{eventhubNamespace}.servicebus.windows.net",
            EventHubName,
            new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                TenantId = config.GetSection("AadTenantId").Value
            }));


        List<string> messages = GetMessages();

        Console.WriteLine($"Trying to send {messages.Count} messages to {EventHubName}...");

        try
        {
            foreach (string message in messages)
            {
                Console.WriteLine($"Sending {message} ...");
                using (EventDataBatch eventBatch = await producerClient.CreateBatchAsync())
                {
                    eventBatch.TryAdd(new EventData()
                    {
                        EventBody = new BinaryData(Encoding.UTF8.GetBytes(message))
                    });

                    await producerClient.SendAsync(eventBatch);
                }
            }
        }
        finally
        {
            await producerClient.CloseAsync();
            await producerClient.DisposeAsync();
        }

        Console.WriteLine($"Send {messages.Count} messages to {EventHubName}.");
    }

    private static List<string> GetMessages()
    {
        return new List<string>()
        {
            """
                {
                    "assetContianerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "bb5ab2dd-f89c-4689-976b-0de2fce614ec",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "walk",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "walk_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "walk_720p.mp4"
                        }
                    ]
                }
             """,
            """
                {
                    "assetContianerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "227ac968-7f98-41b5-806c-cd966f41128c",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "beach",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "beach_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "beach_720p.mp4"
                        }
                    ]
                }
             """
            //,
            // """
            //     {
            //         "assetContianerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
            //         "assetId": "a3a45e98-a2ce-4b37-84ab-3fbd1cae624f",
            //         "originalAssetBlobName": "original",
            //         "sourceStorageAccount": "cheuw001assetsstcool",
            //         "destinationStorageAccount": "cheuw001assetssthot",
            //         "outFilePrefix": "friendseating",
            //         "commandArgs": [
            //             {
            //             "outFileOptions": "-vf fps=1/4",
            //             "outFileName": "friendseating_%04d.png"
            //             },
            //             {
            //             "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
            //             "outFileName": "friendseating_720p.mp4"
            //             }
            //         ]
            //     }
            //  """,
            // """
            //     {
            //         "assetContianerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
            //         "assetId": "1e049c87-ce56-4c54-afc8-0c5a01a97bf3",
            //         "originalAssetBlobName": "original",
            //         "sourceStorageAccount": "cheuw001assetsstcool",
            //         "destinationStorageAccount": "cheuw001assetssthot",
            //         "outFilePrefix": "boatnewyork",
            //         "commandArgs": [
            //             {
            //             "outFileOptions": "-vf fps=1/4",
            //             "outFileName": "boatnewyork_%04d.png"
            //             },
            //             {
            //             "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
            //             "outFileName": "boatnewyork_720p.mp4"
            //             }
            //         ]
            //     }
            // """,
            // """
            //     {
            //         "assetContianerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
            //         "assetId": "66403f95-2ded-496c-bb50-38713bbe2000",
            //         "originalAssetBlobName": "original",
            //         "sourceStorageAccount": "cheuw001assetsstcool",
            //         "destinationStorageAccount": "cheuw001assetssthot",
            //         "outFilePrefix": "waterstream",
            //         "commandArgs": [
            //             {
            //             "outFileOptions": "-vf fps=1/2",
            //             "outFileName": "waterstream_%04d.png"
            //             },
            //             {
            //             "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
            //             "outFileName": "waterstream_720p.mp4"
            //             }
            //         ]
            //     }
            // """
        };
    }
}
