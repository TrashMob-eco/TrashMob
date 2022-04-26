
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Persistence;

    public class UpcomingEventAttendingThisWeekNotifier : UpcomingEventAttendingBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType { get { return NotificationTypeEnum.UpcomingEventAttendingThisWeek; } }

        protected override int NumberOfHoursInWindow => 7 * 24;

        protected override string EmailSubject => "You're attending a TrashMob.eco event this week!";

        public UpcomingEventAttendingThisWeekNotifier(IEventRepository eventRepository, 
                                                      IUserRepository userRepository, 
                                                      IEventAttendeeRepository eventAttendeeRepository,
                                                      IUserNotificationRepository userNotificationRepository,
                                                      IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
                                                      IEmailSender emailSender,
                                                      IEmailManager emailManager,
                                                      IMapRepository mapRepository,
                                                      ILogger logger) :
            base(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, emailManager, mapRepository, logger)
        {
        }
    }
}
