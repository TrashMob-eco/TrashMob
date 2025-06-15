namespace TrashMobJobs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Persistence;
    using Microsoft.Extensions.Azure;
    using Azure.Identity;
    using System.Dynamic;

    public class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(c => { c.AddEnvironmentVariables(); })
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services =>
                {
                    ServiceBuilder.AddManagers(services);
                    ServiceBuilder.AddRepositories(services);
                    services.AddScoped<IUserNotificationManager, TrashMob.Shared.Engine.UserNotificationManager>();
                    services.AddDbContext<MobDbContext>();

                    Uri blobStorageUrl = new(Environment.GetEnvironmentVariable("StorageAccountUri") ??
                        throw new InvalidOperationException("The environment variable 'StorageAccountUri' is not set or is empty."));

                    var environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") ??
                        throw new InvalidOperationException("The environment variable 'AZURE_FUNCTIONS_ENVIRONMENT' is not set or is empty.");

                    if (environment == "Local")
                    {
                        string tenantId = new(Environment.GetEnvironmentVariable("TrashMobBackendTenantId") ??
                            throw new InvalidOperationException("The environment variable 'TrashMobBackendTenantId' is not set or is empty."));

                        services.AddScoped<IKeyVaultManager, LocalKeyVaultManager>();
                        services.AddAzureClients(azureClientFactoryBuilder =>
                        {
                            azureClientFactoryBuilder.UseCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions
                            {
                                VisualStudioTenantId = tenantId
                            }));
                            azureClientFactoryBuilder.AddBlobServiceClient(blobStorageUrl);
                        });
                    }
                    else
                    {
                        services.AddAzureClients(azureClientFactoryBuilder =>
                        {
                            azureClientFactoryBuilder.UseCredential(new DefaultAzureCredential());
                            Uri vaultUri = new(Environment.GetEnvironmentVariable("VaultUri") ??
                                throw new InvalidOperationException("The environment variable 'VaultUri' is not set or is empty."));

                            azureClientFactoryBuilder.AddSecretClient(vaultUri);
                            azureClientFactoryBuilder.AddBlobServiceClient(blobStorageUrl);
                        });

                        services.AddScoped<IKeyVaultManager, KeyVaultManager>();
                    }
                })
                .Build();

            await host.RunAsync().ConfigureAwait(false);
        }
    }
}