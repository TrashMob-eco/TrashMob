
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UpcomingEventAttendingThisWeekNotifier : UpcomingEventAttendingBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType { get { return NotificationTypeEnum.UpcomingEventAttendingThisWeek; } }

        protected override int NumberOfHoursInWindow => 7 * 24;

        protected override string EmailSubject => "You're attending a TrashMob.eco event this week!";

        public UpcomingEventAttendingThisWeekNotifier(IEventRepository eventRepository, 
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
