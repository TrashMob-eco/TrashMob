
namespace TrashMob.Shared.Engine
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence;

    public class UpcomingEventsInYourAreaTodayNotifier : NotificationEngineBase, INotificationEngine
    {
        public UpcomingEventsInYourAreaTodayNotifier(IEventRepository eventRepository, IUserRepository userRepository, IEventAttendeeRepository eventAttendeeRepository) : base(eventRepository, userRepository, eventAttendeeRepository)
        {
        }

        public Task GenerateNotificationsAsync()
        {
            return Task.CompletedTask;
        }
    }
}
