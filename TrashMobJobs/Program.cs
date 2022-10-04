
namespace TrashMobJobs
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using TrashMob.Shared;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Engine;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Persistence.Events;
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
                     .AddSingleton<IEventAttendeeRepository, EventAttendeeRepository>()
                     .AddSingleton<IEventRepository, EventRepository>()
                     .AddSingleton<ILookupRepository<EventStatus>, LookupRepository<EventStatus>>()
                     .AddSingleton<IBaseManager<EventSummary>, EventSummaryManager>()
                     .AddSingleton<ILookupRepository<EventType>, LookupRepository<EventType>>()
                     .AddSingleton<IMapRepository, MapRepository>()
                     .AddSingleton<IKeyedRepository<UserNotification>, KeyedRepository<UserNotification>>()
                     .AddSingleton<IKeyedRepository<NonEventUserNotification>, KeyedRepository<NonEventUserNotification>>())
                .Build();

            await host.RunAsync().ConfigureAwait(false);
        }
    }
}