using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace common.lib.Configs
{
    public class ConfigLoader
    {
        public static IConfigurationRoot LoadConfiguration(IConfigurationBuilder configBuilder)
        {
            configBuilder.AddJsonFile(Environment.GetEnvironmentVariable("CH_VIDEO_WI_CONFIG-av"));

            var config = configBuilder.Build();


            string? appConfigEndpont = config.GetSection("AppConfigEndpoint").Value;
            string? appConfigLabel = config.GetSection("AppConfigLabel").Value;
            string? sharedAppConfiglabel = config.GetSection("SharedAppConfiglabel").Value;
            string? keyVaultName = config.GetSection("KeyVaultName").Value;
            string? aadTenantId = config.GetSection("AadTenantId").Value;


            //Load configuration from Azure App Configuration
            configBuilder.AddAzureAppConfiguration(options =>
            {
                DefaultAzureCredential azureCredentials = new(
                    // Provide tenant id as shown below 
                    // or specify environment variable AZURE_TENANT_ID
                    // or VS if you are logged into an Azure AD account having access to app config and key vault that would be sufficient
                    // or in vscode terminal az login and set subscription to a subscription in the tenant you want to use before "dotnet run"
                    new DefaultAzureCredentialOptions
                    {
                        TenantId = aadTenantId
                    });
                options.Connect(
                    new Uri(appConfigEndpont),
                    azureCredentials);

                options
                        .Select(KeyFilter.Any, sharedAppConfiglabel)
                        .Select(KeyFilter.Any, appConfigLabel);

                SecretClient secretClient = new(
                    new Uri($"https://{keyVaultName}.vault.azure.net/"),
                    azureCredentials);

                options.ConfigureKeyVault(kv =>
                    kv.Register(secretClient));
            });

            return configBuilder.Build();
        }
    }
}
