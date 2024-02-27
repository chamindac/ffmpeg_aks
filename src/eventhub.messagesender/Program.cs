using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using common.lib.Configs;

namespace eventhub.messagesender;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = new HostBuilder();

        builder.ConfigureAppConfiguration((config) =>
        {
            ConfigLoader.LoadConfiguration(config);
        });

        builder.ConfigureLogging((context, b) =>
        {
            b.AddConsole();
        });

        //builder.ConfigureWebJobs(b =>
        //{
        //    b.AddEventHubs();
        //});

        var host = builder.Build();

        using (host)
        {
            await host.RunAsync();
        }
    }
}
