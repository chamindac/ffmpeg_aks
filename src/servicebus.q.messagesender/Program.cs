// name of your Service Bus queue
// the client that owns the connection and can be used to create senders and receivers
using Azure.Identity;
using Azure.Messaging.ServiceBus;

ServiceBusClient client;

// the sender used to publish messages to the queue
ServiceBusSender sender;

// The Service Bus client types are safe to cache and use as a singleton for the lifetime
// of the application, which is best practice when messages are being published or read
// regularly.
//
// Set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443. 
// If you use the default AmqpTcp, ensure that ports 5671 and 5672 are open.
var clientOptions = new ServiceBusClientOptions
{
    TransportType = ServiceBusTransportType.AmqpWebSockets
};
//TODO: Replace the "<NAMESPACE-NAME>" and "<QUEUE-NAME>" placeholders.
client = new ServiceBusClient(
    "ch-video-dev-euw-001-sbus-blue.servicebus.windows.net",
    new DefaultAzureCredential(
        new DefaultAzureCredentialOptions
        {
            TenantId = "tenantid"
        }
        ),
    clientOptions);
sender = client.CreateSender("dotnetvideoqueue");


var messageTask1 =
    sender.SendMessageAsync(new ServiceBusMessage (body: """
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
    """)
{
    MessageId = Guid.NewGuid().ToString(),
    ContentType = "application/json"
});

//await sender.SendMessageAsync(message);

var messageTask2 =
    sender.SendMessageAsync(new ServiceBusMessage (body: """
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
    """)
{
    MessageId = Guid.NewGuid().ToString(),
    ContentType = "application/json"
});

var messageTask3 =
    sender.SendMessageAsync(new ServiceBusMessage(body: """
    {
        "assetContianerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
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
    """)
{
    MessageId = Guid.NewGuid().ToString(),
    ContentType = "application/json"
});

var messageTask4 =
    sender.SendMessageAsync(new ServiceBusMessage(body: """
    {
        "assetContianerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
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
    """)
{
    MessageId = Guid.NewGuid().ToString(),
    ContentType = "application/json"
});


var messageTask5 =
    sender.SendMessageAsync(new ServiceBusMessage(body: """
    {
        "assetContianerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
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
    """)
{
    MessageId = Guid.NewGuid().ToString(),
    ContentType = "application/json"
});

await Task.WhenAll(messageTask1, messageTask2, messageTask3, messageTask4, messageTask5);


Console.WriteLine($"Sent 5 messages");

await sender.DisposeAsync();
await client.DisposeAsync();