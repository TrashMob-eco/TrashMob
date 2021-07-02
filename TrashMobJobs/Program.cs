
namespace TrashMobJobs
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using TrashMob.Shared;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Engine;
    using System.Threading.Tasks;

    public class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(s =>
                    s.AddSingleton<IEmailSender, EmailSender>()
                     .AddDbContext<MobDbContext>()
                     .AddSingleton<IContactRequestRepository, ContactRequestRepository>()
                     .AddSingleton<IEmailManager, EmailManager>()
                     .AddSingleton<IEventAttendeeRepository, EventAttendeeRepository>()
                     .AddSingleton<IEventRepository, EventRepository>()
                     .AddSingleton<IEventStatusRepository, EventStatusRepository>()
                     .AddSingleton<IEventTypeRepository, EventTypeRepository>()
                     .AddSingleton<IMapRepository, MapRepository>()
                     .AddSingleton<IUserRepository, UserRepository>()
                     .AddSingleton<IUserNotificationRepository, UserNotificationRepository>()
                     .AddSingleton<IUserNotificationPreferenceRepository, UserNotificationPreferenceRepository>()
                     .AddSingleton<IUserNotificationManager, UserNotificationManager>())
                .Build();

            await host.RunAsync().ConfigureAwait(false);
        }
    }
}