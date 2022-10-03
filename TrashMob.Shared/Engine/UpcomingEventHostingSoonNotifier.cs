
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UpcomingEventHostingSoonNotifier : UpcomingEventHostingBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventHostingSoon;

        protected override int NumberOfHoursInWindow => 24;

        protected override string EmailSubject => "You're hosting a TrashMob.eco event soon!";

        public UpcomingEventHostingSoonNotifier(IEventRepository eventRepository, 
                                                IKeyedManager<User> userManager, 
                                                IEventAttendeeRepository eventAttendeeRepository, 
                                                IKeyedManager<UserNotification> userNotificationManager,
                                                IKeyedManager<NonEventUserNotification> nonEventUserNotificationManager,
                                                IEmailSender emailSender,
                                                IEmailManager emailManager,
                                                IMapRepository mapRepository,
                                                ILogger logger) :
            base(eventRepository, userManager, eventAttendeeRepository, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger)
        {
        }
    }
}
