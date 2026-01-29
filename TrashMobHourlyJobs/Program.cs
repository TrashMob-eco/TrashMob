namespace TrashMobHourlyJobs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Persistence;
    using Microsoft.Extensions.Azure;
    using Azure.Identity;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var userNotificationManager = scope.ServiceProvider.GetRequiredService<IUserNotificationManager>();

            logger.LogInformation("UserNotifier job started at: {Time}", DateTime.UtcNow);

            await userNotificationManager.RunAllNotifications();

            logger.LogInformation("UserNotifier job completed at: {Time}", DateTime.UtcNow);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Build configuration from environment variables
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

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
        }
    }
}
