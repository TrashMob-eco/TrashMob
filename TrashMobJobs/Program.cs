
namespace TrashMobJobs
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using TrashMob.Shared;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Engine;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Persistence.Events;

    public class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(c =>
                {
                    c.AddEnvironmentVariables();
                })
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(s =>
                    s.AddSingleton<IEmailSender, EmailSender>()
                     .AddDbContext<MobDbContext>()
                     .AddSingleton<IEmailManager, EmailManager>()
                     .AddSingleton<IEventAttendeeRepository, EventAttendeeRepository>()
                     .AddSingleton<IEventRepository, EventRepository>()
                     .AddSingleton<IEventStatusRepository, EventStatusRepository>()
                     .AddSingleton<IEventSummaryRepository, EventSummaryRepository>()
                     .AddSingleton<IEventTypeRepository, EventTypeRepository>()
                     .AddSingleton<IMapRepository, MapRepository>()
                     .AddSingleton<IUserRepository, UserRepository>()
                     .AddSingleton<IUserNotificationRepository, UserNotificationRepository>()
                     .AddSingleton<INonEventUserNotificationRepository, NonEventUserNotificationRepository>()
                     .AddSingleton<IUserNotificationManager, UserNotificationManager>())
                .Build();

            await host.RunAsync().ConfigureAwait(false);
        }
    }
}