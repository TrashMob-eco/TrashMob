namespace TrashMob.Shared.Engine
{
    using TrashMob.Shared.Persistence;

    public abstract class NotificationEngineBase
    {
        private readonly IEventRepository eventRepository;
        private readonly IUserRepository userRepository;
        private readonly IEventAttendeeRepository eventAttendeeRepository;

        public NotificationEngineBase(IEventRepository eventRepository, IUserRepository userRepository, IEventAttendeeRepository eventAttendeeRepository)
        {
            this.eventRepository = eventRepository;
            this.userRepository = userRepository;
            this.eventAttendeeRepository = eventAttendeeRepository;
        }
    }
}
