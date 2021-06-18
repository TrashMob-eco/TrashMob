namespace TrashMob.Shared.Engine
{
    using System.Threading.Tasks;

    public class NotificationEngine : INotificationEngine
    {
        public Task GenerateEventNotificatonsAsync()
        {
            return Task.CompletedTask;
        }
    }
}
