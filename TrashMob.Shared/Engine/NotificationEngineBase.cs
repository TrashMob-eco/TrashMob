namespace TrashMob.Shared.Engine
{
    using System.IO;
    using System.Reflection;
    using TrashMob.Shared.Persistence;

    public abstract class NotificationEngineBase
    {
        protected IEventRepository EventRepository { get; }

        protected IUserRepository UserRepository { get; }
        
        protected IEventAttendeeRepository EventAttendeeRepository { get; }

        protected IUserNotificationRepository UserNotificationRepository { get; }

        protected IEmailSender EmailSender { get; }

        protected abstract NotificationTypeEnum NotificationType { get; }

        protected abstract string EmailSubject { get; }

        protected string SendGridApiKey { get; }

        public NotificationEngineBase(IEventRepository eventRepository, 
                                      IUserRepository userRepository, 
                                      IEventAttendeeRepository eventAttendeeRepository, 
                                      IUserNotificationRepository userNotificationRepository, 
                                      IEmailSender emailSender)
        {
            EventRepository = eventRepository;
            UserRepository = userRepository;
            EventAttendeeRepository = eventAttendeeRepository;
            UserNotificationRepository = userNotificationRepository;
            EmailSender = emailSender;
        }

        public string GetEmailTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("TrashMob.Shared.Engine.EmailTemplates.{0}.html", NotificationType);
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
