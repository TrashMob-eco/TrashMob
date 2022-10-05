
namespace TrashMobJobs
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using TrashMob.Shared;
    using TrashMob.Shared.Persistence;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Models;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.Events;

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
                     .AddSingleton<IEventAttendeeManager, EventAttendeeManager>()
                     .AddSingleton<IEventManager, EventManager>()
                     .AddSingleton<ILookupRepository<EventStatus>, LookupRepository<EventStatus>>()
                     .AddSingleton<IBaseManager<EventSummary>, EventSummaryManager>()
                     .AddSingleton<ILookupRepository<EventType>, LookupRepository<EventType>>()
                     .AddSingleton<IMapManager, MapManager>()
                     .AddSingleton<IKeyedRepository<UserNotification>, KeyedRepository<UserNotification>>()
                     .AddSingleton<IKeyedRepository<NonEventUserNotification>, KeyedRepository<NonEventUserNotification>>())
                .Build();

            await host.RunAsync().ConfigureAwait(false);
        }
    }
}