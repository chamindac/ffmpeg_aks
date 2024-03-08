using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using common.lib.Configs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs;
using Azure.Storage.Blobs;
using System.Text;

namespace videoprocessor.eventhub;

internal class Program
{
    private const string EventHubName = "videopreview";
    private const string EventHubConsumer = "videoprevieweventhandler";

    static async Task Main(string[] args)
    {
        //using (IHost host = CreateHostBuilder().Build())
        //{
        //    await host.RunAsync();
        //}

        IConfigurationRoot config = ConfigLoader.LoadConfiguration(new ConfigurationBuilder());

        DefaultAzureCredential azureCredentials = new DefaultAzureCredential(
                new DefaultAzureCredentialOptions
                {
                    TenantId = config["AadTenantId"]
                });
        string eventhubNamespace = config["EventHubNamespaceName-1"];

        BlobContainerClient storageClient = new BlobContainerClient(
        new Uri($"https://{config["EventHubStorageName"]}.blob.core.windows.net/{EventHubConsumer}-{EventHubName}"),
        azureCredentials);

        var processor = new EventProcessorClient(
            storageClient,
            EventHubConsumer,
            $"{eventhubNamespace}.servicebus.windows.net",
            $"{EventHubName}",
            azureCredentials);

        // Register handlers for processing events and handling errors
        processor.ProcessEventAsync += ProcessEventHandler;
        processor.ProcessErrorAsync += ProcessErrorHandler;

        // Start the processing
        await processor.StartProcessingAsync();
        Console.WriteLine("start processing");

        // Wait for 30 seconds for the events to be processed
        await Task.Delay(TimeSpan.FromSeconds(10));
        Console.WriteLine("Delay is over");

        // Stop the processing
        await processor.StopProcessingAsync();
        Console.WriteLine("stop processing");

        Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            // Write the body of the event to the console window
            Console.WriteLine("\tReceived event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
            Console.ReadLine();
            return Task.CompletedTask;
        }

        Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            Console.ReadLine();
            return Task.CompletedTask;
        }

    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .UseConsoleLifetime()
            .ConfigureAppConfiguration((config) =>
            {
                ConfigLoader.LoadConfiguration(config);
            })
            .ConfigureServices((_, services) =>
            {
                services.AddHostedService<ScaledJobHostedService>();
            })
            .ConfigureLogging((_, logging) =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options => options.IncludeScopes = true);
            });
    }
}