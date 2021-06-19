namespace TrashMob.Shared.Engine
{
    using System.Threading.Tasks;

    public class NotificationEngine : INotificationEngine
    {
        private readonly IEventManager eventManager;

        public NotificationEngine(IEventManager eventManager)
        {
            this.eventManager = eventManager;
        }

        public Task GenerateEventNotificatonsAsync()
        {

            return Task.CompletedTask;
        }
    }
}
