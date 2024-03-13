using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using videoprocessor.eventhub.Interfaces;
using videoprocessor.eventhub.Models;

namespace videoprocessor.eventhub;

internal sealed class ScaledJobHostedService : BackgroundService
{
    private const string EventHubName = "videopreview";
    private const string EventHubConsumer = "videoprevieweventhandler";
    private const int TerminationGracePeriodSeconds = 60;

    private readonly ILogger<ScaledJobHostedService> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IConfiguration _configuration;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IVideoTranscoder _videoTranscorder;

    private int _processingEventCount;

    public ScaledJobHostedService(
        ILogger<ScaledJobHostedService> logger,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        IVideoTranscoder videoTranscoder)
    {
        _logger = logger;
        _applicationLifetime = applicationLifetime;
        _configuration = configuration;
        _videoTranscorder = videoTranscoder;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string eventhubNamespace = _configuration["EventHubNamespaceName-1"];
        _logger.LogInformation($"Scaled job started for {eventhubNamespace}.");

        DefaultAzureCredential azureCredentials = new DefaultAzureCredential(
                new DefaultAzureCredentialOptions
                {
                    TenantId = _configuration["AadTenantId"]
                });

        BlobContainerClient storageClient = new (
        new Uri($"https://{_configuration["EventHubStorageName"]}.blob.core.windows.net/{EventHubConsumer}-{EventHubName}"),
        azureCredentials);

        EventProcessorClient eventProcessorClient = new (
            storageClient,
            EventHubConsumer,
            $"{eventhubNamespace}.servicebus.windows.net",
            $"{EventHubName}",
            azureCredentials);

        // Register handlers for processing events and handling errors
        eventProcessorClient.ProcessEventAsync += VideoTranscordEventHandlerAsync;
        eventProcessorClient.ProcessErrorAsync += VideoTranscordEventErrorHandlerAsync;
        _processingEventCount = 0;

        try
        {
            // Start the processing
            await eventProcessorClient.StartProcessingAsync(_cancellationTokenSource.Token);

            _logger.LogInformation($"Scaled job started for {EventHubConsumer}-{EventHubName} event processing...");
            await Task.Delay(Timeout.Infinite, _cancellationTokenSource.Token);

        }
        catch (TaskCanceledException)
        {
            // This is expected if the cancellation token is signaled.
        }
        finally
        {
            _logger.LogInformation("Scaled job terminating...");
            // Stop the processing
            await eventProcessorClient.StopProcessingAsync();
            eventProcessorClient.ProcessEventAsync -= VideoTranscordEventHandlerAsync;
            eventProcessorClient.ProcessErrorAsync -= VideoTranscordEventErrorHandlerAsync;
            _logger.LogInformation("Scaled job terminated.");

            _cancellationTokenSource.Dispose();
            _applicationLifetime.StopApplication();
        }
    }

    private async Task VideoTranscordEventHandlerAsync(ProcessEventArgs eventArgs)
    {
        if (eventArgs.CancellationToken.IsCancellationRequested)
        {
            return;
        }

        _processingEventCount++;
        _logger.LogInformation("Scaled job processing an event...");

        // Write the body of the event to the console window
        await eventArgs.UpdateCheckpointAsync(); // update checkpoint so we mark the message is processed
        _logger.LogInformation("\tReceived event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
        _logger.LogInformation("Processing...");

        await _videoTranscorder.TranscodeAsync(
            eventArgs.Data.EventBody.ToObjectFromJson<TranscodeRequest>(new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

        if (_processingEventCount > 0)
        {
            _processingEventCount--;
        }

        _ = JobTerminationHandlerAsync();
    }

    private Task VideoTranscordEventErrorHandlerAsync(ProcessErrorEventArgs eventArgs)
    {
        // Write details about the error to the console window
        _logger.LogInformation($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
        _logger.LogInformation(eventArgs.Exception.Message);
        return Task.CompletedTask;
    }

    private async Task JobTerminationHandlerAsync()
    {
        if (_processingEventCount > 0)
        {
            _logger.LogInformation($"Scaled job is not termnating due to {_processingEventCount} processing events...");
            return;
        }

        _logger.LogInformation("Scaled job termination Intiated...");
        int secodsElapsed = 0;

        while (_processingEventCount == 0 && secodsElapsed < TerminationGracePeriodSeconds)
        {
            await Task.Delay(1000);
            secodsElapsed++;
            if (_processingEventCount == 0)
            {
                _logger.LogInformation($"Scaled job termination intiated. Elapsed {secodsElapsed} seconds out of {TerminationGracePeriodSeconds} ...");
            }
        }

        if (_processingEventCount == 0)
        {
            _logger.LogInformation("Scaled job termination grace period passed...");
            await _cancellationTokenSource.CancelAsync();
            _logger.LogInformation("Scaled job termination in progress...");
        }
        else
        {
            _logger.LogInformation("Scaled job termination aborted. Continue processing events...");
        }
    }
}