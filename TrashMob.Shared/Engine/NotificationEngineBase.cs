namespace TrashMob.Shared.Engine
{
    using TrashMob.Shared.Persistence;

    public abstract class NotificationEngineBase
    {
        private readonly IEventRepository eventRepository;
        private readonly IUserRepository userRepository;
        private readonly IEventAttendeeRepository eventAttendeeRepository;
        private readonly IEmailSender emailSender;

        public NotificationEngineBase(IEventRepository eventRepository, IUserRepository userRepository, IEventAttendeeRepository eventAttendeeRepository, IEmailSender emailSender)
        {
            this.eventRepository = eventRepository;
            this.userRepository = userRepository;
            this.eventAttendeeRepository = eventAttendeeRepository;
            this.emailSender = emailSender;
        }
    }
}
