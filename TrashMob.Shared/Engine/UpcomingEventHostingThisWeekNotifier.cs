
namespace TrashMob.Shared.Engine
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence;

    public class UpcomingEventHostingThisWeekNotifier : NotificationEngineBase, INotificationEngine
    {
        public UpcomingEventHostingThisWeekNotifier(IEventRepository eventRepository, IUserRepository userRepository, IEventAttendeeRepository eventAttendeeRepository) : base(eventRepository, userRepository, eventAttendeeRepository)
        {
        }

        public Task GenerateNotificationsAsync()
        {
            return Task.CompletedTask;
        }
    }
}
