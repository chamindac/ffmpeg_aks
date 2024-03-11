using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

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

    private bool _terminateIntiated;
    private int _runningJobCount;
    
    public ScaledJobHostedService(
        ILogger<ScaledJobHostedService> logger,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration)
    {
        _logger = logger;
        _applicationLifetime = applicationLifetime;
        _configuration = configuration;
        _cancellationTokenSource = new CancellationTokenSource();
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
        _terminateIntiated = false;

        try
        {
            // Start the processing
            await processor.StartProcessingAsync();

            _logger.LogInformation($"Event handler job started for {EventHubConsumer}-{EventHubName} event processing...");
            await Task.Delay(Timeout.Infinite, _cancellationTokenSource.Token);

        }
        catch (TaskCanceledException)
        {
            // This is expected if the cancellation token is signaled.
        }
        finally
        {
            _logger.LogInformation("Event handler job terminating...");
            // Stop the processing
            await processor.StopProcessingAsync();
            processor.ProcessEventAsync -= ProcessEventHandler;
            processor.ProcessErrorAsync -= ProcessErrorHandler;
            _logger.LogInformation("Event handler job terminated.");

            _cancellationTokenSource.Dispose();
            _applicationLifetime.StopApplication();
        }
    }

    private async Task HandleTerminationAsync()
    {
        if (_runningJobCount > 0)
        {
            _logger.LogInformation($"Event handler job is not termnating due to {_runningJobCount} running events...");
            return;
        }

        _terminateIntiated = true;
        _logger.LogInformation("Event handler job termination Intiated...");
        int secodsElapsed = 0;

        while (_terminateIntiated && secodsElapsed < TerminationGracePeriodSeconds)
        {
            await Task.Delay(1000);
            secodsElapsed++;
            if (_terminateIntiated)
            {
                _logger.LogInformation($"Event handler job termination intiated. Elapsed {secodsElapsed} seconds out of {TerminationGracePeriodSeconds} ...");
            }
        }

        if (_terminateIntiated)
        {
            _logger.LogInformation("Event handler job termination grace period passed...");
            await _cancellationTokenSource.CancelAsync();
            _logger.LogInformation("Event handler job termination in progress...");
        }
        else
        {
            _logger.LogInformation("Event handler job termination aborted. Continue processing events...");
        }
    }

    private async Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        _terminateIntiated = false;
        _runningJobCount++;
        _logger.LogInformation("Event handler job processing an event...");

        // Write the body of the event to the console window
        await eventArgs.UpdateCheckpointAsync(); // update checkpoint so we mark the message is processed
        _logger.LogInformation("\tReceived event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
        _logger.LogInformation("Processing...");
        await Task.Delay(TimeSpan.FromSeconds(10)); // This where we call video process service to generate previews

        if (_runningJobCount > 0) { _runningJobCount--; }
        HandleTerminationAsync();
    }

    private Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        // Write details about the error to the console window
        _logger.LogInformation($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
        _logger.LogInformation(eventArgs.Exception.Message);
        return Task.CompletedTask;
    }
}