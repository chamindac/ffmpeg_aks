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

internal sealed class VideoProcessorHostedService : BackgroundService
{
    private const string EventHubName = "videopreview";
    private const string EventHubConsumer = "videoprevieweventhandler";
    //private const int TerminationGracePeriodSeconds = 60;

    private readonly ILogger<VideoProcessorHostedService> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IVideoTranscoder _videoTranscorder;
    private readonly EventProcessorClient _eventProcessorClient;

    private int _processingEventCount;

    public VideoProcessorHostedService(
        ILogger<VideoProcessorHostedService> logger,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        IVideoTranscoder videoTranscoder)
    {
        _logger = logger;
        _applicationLifetime = applicationLifetime;
        _videoTranscorder = videoTranscoder;
        _cancellationTokenSource = new CancellationTokenSource();

        string eventhubNamespace = configuration["EventHubNamespaceName-1"];
        _logger.LogInformation($"Hosted service started for {eventhubNamespace}.");

        DefaultAzureCredential azureCredentials = new DefaultAzureCredential(
                new DefaultAzureCredentialOptions
                {
                    TenantId = configuration["AadTenantId"]
                });

        BlobContainerClient storageClient = new(
        new Uri($"https://{configuration["EventHubStorageName"]}.blob.core.windows.net/{EventHubConsumer}-{EventHubName}"),
        azureCredentials);

        _eventProcessorClient = new(
            storageClient,
            EventHubConsumer,
            $"{eventhubNamespace}.servicebus.windows.net",
            $"{EventHubName}",
            azureCredentials);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Register handlers for processing events and handling errors
        _eventProcessorClient.ProcessEventAsync += VideoTranscordEventHandlerAsync;
        _eventProcessorClient.ProcessErrorAsync += VideoTranscordEventErrorHandlerAsync;
        _processingEventCount = 0;

        try
        {
            // Start the processing
            await _eventProcessorClient.StartProcessingAsync(_cancellationTokenSource.Token);

            _logger.LogInformation($"Hosted service started for {EventHubConsumer}-{EventHubName} event processing...");
            await Task.Delay(Timeout.Infinite, _cancellationTokenSource.Token);

        }
        catch (TaskCanceledException)
        {
            // This is expected if the cancellation token is signaled.
        }
        finally
        {
            _applicationLifetime.StopApplication();
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cancellationTokenSource.CancelAsync();
        _logger.LogInformation($"Hosted service termination intiated, waiting for {_processingEventCount} processing events...");
        // Event processor client will wait, for current processing events to be completed, before stopping.
        // No new events will be recieved, since stop is triggered.
        // StopProcessingAsync is equivalent to dispose (there is no IDisposable implmentation in EventProcessorClient. More information: https://learn.microsoft.com/en-us/dotnet/api/azure.messaging.eventhubs.eventprocessorclient?view=azure-dotnet#remarks).

        await _eventProcessorClient.StopProcessingAsync(cancellationToken);

        _logger.LogInformation($"Processing events count is {_processingEventCount}. Hosted service terminating...");
        _eventProcessorClient.ProcessEventAsync -= VideoTranscordEventHandlerAsync;
        _eventProcessorClient.ProcessErrorAsync -= VideoTranscordEventErrorHandlerAsync;
        _cancellationTokenSource.Dispose();
        _logger.LogInformation("Scaled job terminated.");

        await base.StopAsync(cancellationToken);
    }

    private async Task VideoTranscordEventHandlerAsync(ProcessEventArgs eventArgs)
    {
        if (eventArgs.CancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"Hosted service rejecting an event. Cancellation token is {eventArgs.CancellationToken.IsCancellationRequested}, in progress events {_processingEventCount}.");
            return;
        }

        if (eventArgs.HasEvent)
        {
            await eventArgs.UpdateCheckpointAsync(); // update checkpoint so we mark the message is processed
            _logger.LogInformation("Updated checkpoint.");

            _processingEventCount++;
            _logger.LogInformation($"Hosted service processing an event... Cancellation token is {eventArgs.CancellationToken.IsCancellationRequested}, in progress events {_processingEventCount}.");

            // Write the body of the event to the console window
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

            //_ = _cancellationTokenSource.CancelAsync();
            //_ = JobTerminationHandlerAsync();
        }
    }

    private Task VideoTranscordEventErrorHandlerAsync(ProcessErrorEventArgs eventArgs)
    {
        // Write details about the error to the console window
        _logger.LogInformation($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered in operation {eventArgs.Operation}. This was not expected to happen.");
        _logger.LogInformation(eventArgs.Exception.Message);
        Exception? innerOne = eventArgs.Exception.InnerException;
        while (innerOne is not null)
        {
            _logger.LogInformation(innerOne.Message);
            innerOne = eventArgs.Exception.InnerException;
        }
        return Task.CompletedTask;
    }

    //private async Task JobTerminationHandlerAsync()
    //{
    //    if (_processingEventCount > 0)
    //    {
    //        _logger.LogInformation($"Hosted service is not termnating due to {_processingEventCount} processing events...");
    //        return;
    //    }

    //    _logger.LogInformation("Hosted service termination Intiated...");
    //    int secodsElapsed = 0;

    //    while (_processingEventCount == 0 && secodsElapsed < TerminationGracePeriodSeconds)
    //    {
    //        await Task.Delay(1000);
    //        secodsElapsed++;
    //        if (_processingEventCount == 0)
    //        {
    //            _logger.LogInformation($"Hosted service termination intiated. Elapsed {secodsElapsed} seconds out of {TerminationGracePeriodSeconds} ...");
    //        }
    //    }

    //    if (_processingEventCount == 0)
    //    {
    //        _logger.LogInformation("Hosted service termination grace period passed...");
    //        await _cancellationTokenSource.CancelAsync();
    //        _logger.LogInformation("Hosted service termination in progress...");
    //    }
    //    else
    //    {
    //        _logger.LogInformation("Hosted service termination aborted. Continue processing events...");
    //    }
    //}
}