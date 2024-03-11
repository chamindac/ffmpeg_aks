using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace videoprocessor.eventhub;

internal sealed class ScaledJobHostedService : BackgroundService
{
    private const string EventHubName = "videopreview";
    private const string EventHubConsumer = "videoprevieweventhandler";
    private const int TerminationGracePeriodSeconds = 60;
    private const int EventProcessCheckIntervalSeconds = 5;

    private readonly ILogger<ScaledJobHostedService> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IConfiguration _configuration;

    private bool _terminateIntiated;
    private bool _continueProcessing;

    public ScaledJobHostedService(
        ILogger<ScaledJobHostedService> logger,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration)
    {
        _logger = logger;
        _applicationLifetime = applicationLifetime;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        string eventhubNamespace = _configuration["EventHubNamespaceName-1"];
        _logger.LogInformation($"Event handler job started for {eventhubNamespace}.");

        DefaultAzureCredential azureCredentials = new DefaultAzureCredential(
                new DefaultAzureCredentialOptions
                {
                    TenantId = _configuration["AadTenantId"]
                });

        BlobContainerClient storageClient = new BlobContainerClient(
        new Uri($"https://{_configuration["EventHubStorageName"]}.blob.core.windows.net/{EventHubConsumer}-{EventHubName}"),
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
        _continueProcessing = true;
        _terminateIntiated = false;

        while (_continueProcessing)
        {
            if (_terminateIntiated)
            {
                _continueProcessing = false;
                _logger.LogInformation("Event handler job termination intiated...");
                await Task.Delay(TimeSpan.FromSeconds(TerminationGracePeriodSeconds));
            }
            else
            {
                // Check event processing status
                await Task.Delay(TimeSpan.FromSeconds(EventProcessCheckIntervalSeconds));
                _logger.LogInformation($"Event handler job running...");
            }
        }

        // Stop the processing
        await processor.StopProcessingAsync();
        processor.ProcessEventAsync -= ProcessEventHandler;
        processor.ProcessErrorAsync -= ProcessErrorHandler;
        _logger.LogInformation("Event handler job terminated.");
        
        _applicationLifetime.StopApplication();
    }

    private async Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        if (_terminateIntiated)
        {
            _continueProcessing = true;
            _terminateIntiated = false;
            _logger.LogInformation("Event handler job termination cancelled...");
        }

        // Write the body of the event to the console window
        await eventArgs.UpdateCheckpointAsync(); // update checkpoint so we mark the message is processed
        Console.WriteLine("\tReceived event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
        Console.WriteLine("Processing...");
        Task.Delay(TimeSpan.FromSeconds(10)); // This where we call video process service to generate previews

        _terminateIntiated = true;
    }

    private Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        // Write details about the error to the console window
        Console.WriteLine($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
        Console.WriteLine(eventArgs.Exception.Message);
        _terminateIntiated = true;
        return Task.CompletedTask;
    }
}