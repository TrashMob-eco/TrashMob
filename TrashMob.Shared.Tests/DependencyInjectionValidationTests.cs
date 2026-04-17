namespace TrashMob.Shared.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Services;
    using Xunit;

    /// <summary>
    /// Validates that the DI container can resolve all registered services.
    /// This catches missing registrations (e.g. AddMemoryCache) that compile
    /// fine but crash at runtime or break EF Core migration tooling.
    /// </summary>
    public class DependencyInjectionValidationTests
    {
        /// <summary>
        /// Builds a DI container mirroring Program.cs registrations and verifies
        /// that all critical application services can be constructed.
        /// </summary>
        [Fact]
        public void AllRegisteredServices_CanBeResolved()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            var builder = Host.CreateApplicationBuilder();

            // Provide minimal config values that Program.cs reads at startup
            builder.Configuration["StorageAccountUri"] = "https://fake.blob.core.windows.net";
            builder.Configuration["TrashMobBackendTenantId"] = Guid.NewGuid().ToString();
            builder.Configuration["AzureAdEntra:Instance"] = "https://login.microsoftonline.com/";
            builder.Configuration["AzureAdEntra:TenantId"] = Guid.NewGuid().ToString();
            builder.Configuration["AzureAdEntra:ClientId"] = Guid.NewGuid().ToString();

            // Register the same services as Program.cs
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpClient("CiamGraph");
            builder.Services.AddSingleton<ICiamGraphService, CiamGraphService>();
            builder.Services.AddHttpClient("Privo");
            builder.Services.AddSingleton<IPrivoService, PrivoService>();
            builder.Services.AddManagers();
            builder.Services.AddRepositories();

            builder.Services.AddAzureClients(azureClientFactoryBuilder =>
            {
                azureClientFactoryBuilder.AddBlobServiceClient(new Uri("https://fake.blob.core.windows.net"));
            });

            builder.Services.AddDbContext<Persistence.MobDbContext>(options =>
                InMemoryDbContextOptionsExtensions.UseInMemoryDatabase(options, "DIValidation"));

            builder.Services.AddScoped<IKeyVaultManager, LocalKeyVaultManager>();

            var host = builder.Build();
            using var scope = host.Services.CreateScope();
            var provider = scope.ServiceProvider;

            var failures = new List<string>();

            // Critical application services that must be resolvable.
            // If you add a new service to Program.cs, add it here too.
            var criticalServices = new[]
            {
                typeof(IPrivoService),
                typeof(ICiamGraphService),
                typeof(IKeyVaultManager),
                typeof(IEventManager),
                typeof(IEmailManager),
                typeof(IMapManager),
                typeof(IUserManager),
                typeof(IPrivoConsentManager),
                typeof(Persistence.MobDbContext),
            };

            foreach (var serviceType in criticalServices)
            {
                try
                {
                    var instance = provider.GetRequiredService(serviceType);
                    Assert.NotNull(instance);
                }
                catch (Exception ex)
                {
                    failures.Add($"{serviceType.Name}: {ex.Message}");
                }
            }

            Assert.True(failures.Count == 0,
                $"Failed to resolve {failures.Count} service(s):\n{string.Join("\n", failures)}");
        }
    }
}
