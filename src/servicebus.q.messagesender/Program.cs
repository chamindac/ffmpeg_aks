// name of your Service Bus queue
// the client that owns the connection and can be used to create senders and receivers
using Azure.Identity;
using Azure.Messaging.ServiceBus;

ServiceBusClient client;

// the sender used to publish messages to the queue
ServiceBusSender sender;

// number of messages to be sent to the queue
const int numOfMessages = 3;

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
    "ch-video-dev-euw-001-sbus.servicebus.windows.net",
    new DefaultAzureCredential(
        new DefaultAzureCredentialOptions
        {
            TenantId = "tenantid"
        }
        ),
    clientOptions);
sender = client.CreateSender("dotnetvideoqueue");

ServiceBusMessage message = new(body: """
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
};

await sender.SendMessageAsync(message);