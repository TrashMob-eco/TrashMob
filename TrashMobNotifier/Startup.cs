namespace TrashMobNotifier
{
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using TrashMob.Shared;
    using TrashMob.Shared.Persistence;

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IEmailSender, EmailSender>();
            builder.Services.AddDbContext<MobDbContext>();
            builder.Services.AddSingleton<IContactRequestRepository, ContactRequestRepository>();
            builder.Services.AddSingleton<IEmailManager, EmailManager>();
            builder.Services.AddSingleton<IEventAttendeeRepository, EventAttendeeRepository>();
            builder.Services.AddSingleton<IEventRepository, EventRepository>();
            builder.Services.AddSingleton<IEventStatusRepository, EventStatusRepository>();
            builder.Services.AddSingleton<IEventTypeRepository, EventTypeRepository>();
            builder.Services.AddSingleton<IMapRepository, MapRepository>();
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            builder.Services.AddSingleton<IUserNotificationRepository, UserNotificationRepository>();
        }
    }
}