
namespace TrashMobJobs
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using TrashMob.Shared;
    using TrashMob.Shared.Persistence;

    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((_, services) =>
                    services.AddSingleton<IEmailSender, EmailSender>()
                            .AddDbContext<MobDbContext>()
                            .AddSingleton<IContactRequestRepository, ContactRequestRepository>()
                            .AddSingleton<IEmailManager, EmailManager>()
                            .AddSingleton<IEventAttendeeRepository, EventAttendeeRepository>()
                            .AddSingleton<IEventRepository, EventRepository>()
                            .AddSingleton<IEventStatusRepository, EventStatusRepository>()
                            .AddSingleton<IEventTypeRepository, EventTypeRepository>()
                            .AddSingleton<IMapRepository, MapRepository>()
                            .AddSingleton<IUserRepository, UserRepository>()
                            .AddSingleton<IUserNotificationRepository, UserNotificationRepository>())
                .Build();

            host.Run();
        }
    }
}