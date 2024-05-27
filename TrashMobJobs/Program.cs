namespace TrashMobJobs
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Persistence;

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
                    services.AddScoped<IUserNotificationManager, UserNotificationManager>();
                    services.AddDbContext<MobDbContext>();
                })
                .Build();

            await host.RunAsync().ConfigureAwait(false);
        }
    }
}