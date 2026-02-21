namespace TrashMobDailyJobs
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

            var statGenerator = scope.ServiceProvider.GetRequiredService<StatGenerator>();
            await statGenerator.RunAsync();

            var leaderboardGenerator = scope.ServiceProvider.GetRequiredService<LeaderboardGenerator>();
            await leaderboardGenerator.RunAsync();

            var achievementProcessor = scope.ServiceProvider.GetRequiredService<AchievementProcessor>();
            await achievementProcessor.RunAsync();
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

            services.AddScoped<StatGenerator>();
            services.AddScoped<LeaderboardGenerator>();
            services.AddScoped<AchievementProcessor>();
            ServiceBuilder.AddManagers(services);
            ServiceBuilder.AddRepositories(services);
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
