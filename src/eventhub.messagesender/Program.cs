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
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
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
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
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
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "a3a45e98-a2ce-4b37-84ab-3fbd1cae624f",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "friendseating",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "friendseating_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "friendseating_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "1e049c87-ce56-4c54-afc8-0c5a01a97bf3",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "boatnewyork",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "boatnewyork_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "boatnewyork_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "66403f95-2ded-496c-bb50-38713bbe2000",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "waterstream",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "waterstream_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "waterstream_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "40f88882-73fd-4785-b662-9c87c2876814",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "strangerthings",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/420",
                        "outFileName": "strangerthings_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "strangerthings_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "693cd80f-e1f0-4d42-b632-88a6829b196d",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "snownight",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "snownight_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "snownight_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "e9e398c9-222e-454d-8e88-c532be6d5842",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "basketball",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "basketball_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "basketball_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "e34a31a8-a169-47cb-b2ac-cf5edcdf444e",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "jump",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "jump_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "jump_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "8299d8a0-0bd2-4024-be05-121edd2bba29",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "kitelaunch",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "kitelaunch_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "kitelaunch_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "048e8847-4eca-490d-a854-e11b14d97483",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "babyandmother",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "babyandmother_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "babyandmother_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "28e35a07-9290-42aa-ab2f-70cfb0598b4c",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "picnic",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "picnic_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "picnic_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "e41e09f4-c912-414b-8f57-ba0fc3de9b27",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "helicopter",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "helicopter_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "helicopter_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "443a0934-b7a8-46ae-b9ac-c9f09943fc93",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "bicycle",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "bicycle_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "bicycle_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "99c8359e-e08c-4222-bac9-8fa79a67e969",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "offroadcar",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "offroadcar_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "offroadcar_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "67eb0e56-2f3e-493d-bb5a-fcc86bf99c63",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "redvantrip",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "redvantrip_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "redvantrip_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "5fbdad39-feb5-4e23-b217-5afa22e47092",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "fastbus",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "fastbus_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "fastbus_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "aa247810-426d-4df1-9718-477153bcd97f",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "buses",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "buses_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "buses_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "f7e333b0-756b-4fe4-9845-cd56bd6f6a00",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "train",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "train_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "train_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "8ff1040f-5d54-4586-8a9d-e8a904697d77",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "redtram",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "redtram_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "redtram_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "c693d32f-8fa3-46d7-a856-a4e15ed68bb2",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "swisstrain",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/3",
                        "outFileName": "swisstrain_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "swisstrain_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "1433e364-a19a-404d-8212-7c2ac151d8f8",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "planelanding",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/3",
                        "outFileName": "planelanding_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "planelanding_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "53b8d27a-d837-4a33-872f-2e5c9c7c6844",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "cruise",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/3",
                        "outFileName": "cruise_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "cruise_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "404179ec-f3ad-48f6-9de0-33b6b176358e",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "skiing",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "skiing_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "skiing_720p.mp4"
                        }
                    ]
                }
            """
        };
    }
}
