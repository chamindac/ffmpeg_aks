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

    private readonly ILogger<ScaledJobHostedService> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IConfiguration _configuration;

    public ScaledJobHostedService(
        ILogger<ScaledJobHostedService> logger,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration)
    {
        _logger = logger;
        _applicationLifetime = applicationLifetime;
        _configuration = configuration;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        string eventhubNamespace = _configuration["EventHubNamespaceName-1"];
        _logger.LogInformation($"Video processing job started for {eventhubNamespace}.");

        _logger.LogInformation($"Video processing job running here.");

        _logger.LogInformation("Video processing job terminated.");
        base.StopAsync(stoppingToken);
        return Task.CompletedTask;

    }
}