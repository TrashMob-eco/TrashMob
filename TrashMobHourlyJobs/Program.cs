namespace TrashMobHourlyJobs
{
    using System;
    using System.Threading.Tasks;
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;

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
            var newsletterManager = scope.ServiceProvider.GetRequiredService<INewsletterManager>();

            logger.LogInformation("Hourly jobs started at: {Time}", DateTime.UtcNow);

            // Run user notifications
            logger.LogInformation("Running user notifications...");
            await userNotificationManager.RunAllNotifications();

            // Process scheduled newsletters
            logger.LogInformation("Processing scheduled newsletters...");
            var scheduledCount = await newsletterManager.ProcessScheduledNewslettersAsync();
            logger.LogInformation("Started sending {Count} scheduled newsletters", scheduledCount);

            // Process newsletters that are in sending status
            logger.LogInformation("Processing sending newsletters...");
            var processedCount = await newsletterManager.ProcessSendingNewslettersAsync();
            logger.LogInformation("Processed {Count} newsletters for sending", processedCount);

            // Process due outreach follow-ups
            logger.LogInformation("Processing prospect outreach follow-ups...");
            var outreachManager = scope.ServiceProvider.GetRequiredService<IProspectOutreachManager>();
            var followUpCount = await outreachManager.ProcessDueFollowUpsAsync();
            logger.LogInformation("Processed {Count} outreach follow-up emails", followUpCount);

            logger.LogInformation("Hourly jobs completed at: {Time}", DateTime.UtcNow);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Build configuration from environment variables + Key Vault secrets
            var configBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                throw new InvalidOperationException("The environment variable 'ASPNETCORE_ENVIRONMENT' is not set or is empty.");

            // In production, load secrets from Azure Key Vault into configuration
            // This makes TMDBServerConnectionString, SendGridApiKey, etc. available via IConfiguration
            if (environment != "Development")
            {
                var vaultUri = Environment.GetEnvironmentVariable("VaultUri") ??
                    throw new InvalidOperationException("The environment variable 'VaultUri' is not set or is empty.");

                var secretClient = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
                configBuilder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
            }

            var configuration = configBuilder.Build();

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
                var vaultUriStr = Environment.GetEnvironmentVariable("VaultUri") ??
                    throw new InvalidOperationException("The environment variable 'VaultUri' is not set or is empty.");

                services.AddAzureClients(azureClientFactoryBuilder =>
                {
                    azureClientFactoryBuilder.UseCredential(new DefaultAzureCredential());
                    azureClientFactoryBuilder.AddSecretClient(new Uri(vaultUriStr));
                    azureClientFactoryBuilder.AddBlobServiceClient(blobStorageUrl);
                });

                services.AddScoped<IKeyVaultManager, KeyVaultManager>();
            }
        }
    }
}
