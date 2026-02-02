namespace TrashMobMCP;

using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using TrashMob.Shared;
using TrashMob.Shared.Managers;
using TrashMob.Shared.Managers.Interfaces;
using TrashMob.Shared.Persistence;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        ConfigureServices(builder.Services);

        // Add MCP server with stdio transport
        builder.Services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();

        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        var host = builder.Build();
        await host.RunAsync();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Add TrashMob managers and repositories
        ServiceBuilder.AddManagers(services);
        ServiceBuilder.AddRepositories(services);
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
