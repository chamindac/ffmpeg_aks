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
    static async Task Main(string[] args)
    {
        using (IHost host = CreateHostBuilder().Build())
        {
            await host.RunAsync();
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