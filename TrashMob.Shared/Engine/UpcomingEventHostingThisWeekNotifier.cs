
namespace TrashMob.Shared.Engine
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence;

    public class UpcomingEventHostingThisWeekNotifier : NotificationEngineBase, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventHostingThisWeek;

        protected override int NumberOfHoursInWindow => 7 * 24;

        protected override string EmailSubject => "Upcoming TrashMob.eco events in your area today!";

        public UpcomingEventHostingThisWeekNotifier(IEventRepository eventRepository, 
                                                    IUserRepository userRepository, 
                                                    IEventAttendeeRepository eventAttendeeRepository,
                                                    IUserNotificationRepository userNotificationRepository,
                                                    IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
                                                    IEmailSender emailSender,
                                                    IMapRepository mapRepository) : 
            base(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, mapRepository)
        {
        }

        public Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
