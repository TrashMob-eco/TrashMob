
namespace TrashMob.Shared.Engine
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence;

    public class UpcomingEventsInYourAreaThisWeekNotifier : NotificationEngineBase, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaThisWeek;

        protected override string EmailSubject => "Upcoming TrashMob.eco events in your area today!";

        public UpcomingEventsInYourAreaThisWeekNotifier(IEventRepository eventRepository, 
                                                        IUserRepository userRepository, 
                                                        IEventAttendeeRepository eventAttendeeRepository,
                                                        IUserNotificationRepository userNotificationRepository,
                                                        IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
                                                        IEmailSender emailSender) : 
            base(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender)
        {
        }

        public Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
