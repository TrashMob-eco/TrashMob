
namespace TrashMob.Shared.Engine
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence;

    public class UpcomingEventAttendingTodayNotifier : NotificationEngineBase, INotificationEngine
    {
        public UpcomingEventAttendingTodayNotifier(IEventRepository eventRepository, IUserRepository userRepository, IEventAttendeeRepository eventAttendeeRepository) : base(eventRepository, userRepository, eventAttendeeRepository)
        {
        }

        public Task GenerateNotificationsAsync()
        {
            return Task.CompletedTask;
        }
    }
}
