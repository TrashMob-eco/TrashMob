namespace TrashMobJobs
{
    using System;
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

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    ServiceBuilder.AddManagers(services);
                    ServiceBuilder.AddRepositories(services);
                    services.AddScoped<IUserNotificationManager, TrashMob.Shared.Engine.UserNotificationManager>();
                    services.AddDbContext<MobDbContext>();

                    Uri blobStorageUrl = new(Environment.GetEnvironmentVariable("StorageAccountUri") ??
                        throw new InvalidOperationException("The environment variable 'StorageAccountUri' is not set or is empty."));

                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                        throw new InvalidOperationException("The environment variable 'ASPNETCORE_ENVIRONMENT' is not set or is empty.");

                    if (environment == "Development")
                    {
                        string tenantId = Environment.GetEnvironmentVariable("TrashMobBackendTenantId") ??
                            throw new InvalidOperationException("The environment variable 'TrashMobBackendTenantId' is not set or is empty.");

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

                    // Register Background Services
                    // Todo... these don't need to background services and should run at different schedules
                    services.AddHostedService<StatGeneratorWorker>();
                    services.AddHostedService<UserNotifierWorker>();
                });
        }
    }
}