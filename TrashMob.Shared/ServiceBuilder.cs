namespace TrashMob.Shared
{
    using Microsoft.Extensions.DependencyInjection;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Managers.Events;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.Partners;
    using TrashMob.Shared.Managers;

    public static class ServiceBuilder
    {
        public static void AddManagers(IServiceCollection services)
        {
            // Migrated Managers
            services.AddScoped<IKeyedManager<ContactRequest>, ContactRequestManager>();
            services.AddScoped<IBaseManager<EventAttendee>, EventAttendeeManager>();
            services.AddScoped<IKeyedManager<Event>, EventManager>();
            services.AddScoped<ILookupManager<EventPartnerLocationStatus>, LookupManager<EventPartnerLocationStatus>>();
            services.AddScoped<IBaseManager<EventPartnerLocation>, EventPartnerLocationManager>();
            services.AddScoped<ILookupManager<EventStatus>, LookupManager<EventStatus>>();
            services.AddScoped<IBaseManager<EventSummary>, EventSummaryManager>();
            services.AddScoped<ILookupManager<EventType>, LookupManager<EventType>>();
            services.AddScoped<IKeyedManager<MessageRequest>, MessageRequestManager>();
            services.AddScoped<IKeyedManager<Partner>, PartnerManager>();
            services.AddScoped<IKeyedManager<PartnerDocument>, PartnerDocumentManager>();
            services.AddScoped<IKeyedManager<PartnerContact>, KeyedManager<PartnerContact>>();
            services.AddScoped<IKeyedManager<PartnerLocation>, KeyedManager<PartnerLocation>>();
            services.AddScoped<IKeyedManager<PartnerLocationContact>, KeyedManager<PartnerLocationContact>>();
            services.AddScoped<IBaseManager<PartnerLocationService>, PartnerLocationServiceManager>();
            services.AddScoped<IKeyedManager<PartnerRequest>, PartnerRequestManager>();
            services.AddScoped<ILookupManager<PartnerRequestStatus>, LookupManager<PartnerRequestStatus>>();
            services.AddScoped<IKeyedManager<PartnerSocialMediaAccount>, PartnerSocialMediaAccountManager>();
            services.AddScoped<ILookupManager<PartnerStatus>, LookupManager<PartnerStatus>>();
            services.AddScoped<ILookupManager<PartnerType>, LookupManager<PartnerType>>();
            services.AddScoped<IBaseManager<PartnerUser>, PartnerUserManager>();
            services.AddScoped<ILookupManager<ServiceType>, LookupManager<ServiceType>>();
            services.AddScoped<ILookupManager<SocialMediaAccountType>, LookupManager<SocialMediaAccountType>>();
            services.AddScoped<IKeyedManager<NonEventUserNotification>, KeyedManager<NonEventUserNotification>>();
            services.AddScoped<IKeyedManager<User>, UserManager>();
            services.AddScoped<IKeyedManager<UserNotification>, UserNotificationManager>();

            // Intentional deviation due to unique methods
            services.AddScoped<IEventAttendeeManager, EventAttendeeManager>();
            services.AddScoped<IEventSummaryManager, EventSummaryManager>();
            services.AddScoped<IEventManager, EventManager>();
            services.AddScoped<IEventPartnerLocationManager, EventPartnerLocationManager>();
            services.AddScoped<IPartnerRequestManager, PartnerRequestManager>();
            services.AddScoped<IPartnerLocationManager, PartnerLocationManager>();
            services.AddScoped<IPartnerLocationContactManager, PartnerLocationContactManager>();
            services.AddScoped<IUserManager, UserManager>();

            // Non-patterned
            services.AddScoped<IDocusignManager, DocusignManager>();
            services.AddScoped<IEmailManager, EmailManager>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IMapManager, MapManager>();
            services.AddScoped<ISecretRepository, SecretRepository>();
            services.AddScoped<INotificationManager, NotificationManager>();
        }

        public static void AddRepositories(IServiceCollection services)
        {
            // Migrated Repositories
            services.AddScoped<IKeyedRepository<ContactRequest>, KeyedRepository<ContactRequest>>();
            services.AddScoped<IKeyedRepository<Event>, KeyedRepository<Event>>();
            services.AddScoped<IBaseRepository<EventAttendee>, BaseRepository<EventAttendee>>();
            services.AddScoped<IBaseRepository<EventPartnerLocation>, BaseRepository<EventPartnerLocation>>();
            services.AddScoped<ILookupRepository<EventPartnerLocationStatus>, LookupRepository<EventPartnerLocationStatus>>();
            services.AddScoped<ILookupRepository<EventStatus>, LookupRepository<EventStatus>>();
            services.AddScoped<IBaseRepository<EventSummary>, BaseRepository<EventSummary>>();
            services.AddScoped<ILookupRepository<EventType>, LookupRepository<EventType>>();
            services.AddScoped<IKeyedRepository<MessageRequest>, KeyedRepository<MessageRequest>>();
            services.AddScoped<IKeyedRepository<NonEventUserNotification>, KeyedRepository<NonEventUserNotification>>();
            services.AddScoped<IKeyedRepository<Partner>, KeyedRepository<Partner>>();
            services.AddScoped<IKeyedRepository<PartnerContact>, KeyedRepository<PartnerContact>>();
            services.AddScoped<IKeyedRepository<PartnerDocument>, KeyedRepository<PartnerDocument>>();
            services.AddScoped<IKeyedRepository<PartnerLocation>, KeyedRepository<PartnerLocation>>();
            services.AddScoped<IKeyedRepository<PartnerLocationContact>, KeyedRepository<PartnerLocationContact>>();
            services.AddScoped<IBaseRepository<PartnerLocationService>, BaseRepository<PartnerLocationService>>();
            services.AddScoped<IKeyedRepository<PartnerRequest>, KeyedRepository<PartnerRequest>>();
            services.AddScoped<ILookupRepository<PartnerRequestStatus>, LookupRepository<PartnerRequestStatus>>();
            services.AddScoped<IKeyedRepository<PartnerSocialMediaAccount>, KeyedRepository<PartnerSocialMediaAccount>>();
            services.AddScoped<ILookupRepository<PartnerStatus>, LookupRepository<PartnerStatus>>();
            services.AddScoped<ILookupRepository<PartnerType>, LookupRepository<PartnerType>>();
            services.AddScoped<IBaseRepository<PartnerUser>, BaseRepository<PartnerUser>>();
            services.AddScoped<ILookupRepository<ServiceType>, LookupRepository<ServiceType>>();
            services.AddScoped<ILookupRepository<SocialMediaAccountType>, LookupRepository<SocialMediaAccountType>>();
            services.AddScoped<IKeyedRepository<User>, KeyedRepository<User>>();
            services.AddScoped<IKeyedRepository<UserNotification>, KeyedRepository<UserNotification>>();
        }
    }
}
