using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using common.lib.Configs;
using Microsoft.Extensions.DependencyInjection;
using videoprocessor.eventhub.Interfaces;
using videoprocessor.eventhub.Services;

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
                services
                .AddScoped<IVideoTranscoder, VideoTranscoder>()
                .AddHostedService<VideoProcessorHostedService>();
            })
            .ConfigureLogging((_, logging) =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options => options.IncludeScopes = true);
            });
    }
}