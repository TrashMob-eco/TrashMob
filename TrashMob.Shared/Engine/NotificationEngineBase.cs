namespace TrashMob.Shared.Engine
{
    using System.IO;
    using System.Reflection;
    using TrashMob.Shared.Persistence;

    public abstract class NotificationEngineBase
    {
        private readonly IEventRepository eventRepository;
        private readonly IUserRepository userRepository;
        private readonly IEventAttendeeRepository eventAttendeeRepository;
        private readonly IEmailSender emailSender;

        protected abstract NotificationTypeEnum NotificationType { get; }

        public NotificationEngineBase(IEventRepository eventRepository, IUserRepository userRepository, IEventAttendeeRepository eventAttendeeRepository, IEmailSender emailSender)
        {
            this.eventRepository = eventRepository;
            this.userRepository = userRepository;
            this.eventAttendeeRepository = eventAttendeeRepository;
            this.emailSender = emailSender;
        }

        public string GetEmailTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"TrashMob.Shared.Engine.EmailTemplates.{NotificationType.ToString()}.html";
            string result;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }
    }
}
