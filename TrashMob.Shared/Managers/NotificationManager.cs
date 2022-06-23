
namespace TrashMob.Shared.Managers
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public class NotificationManager : INotificationManager
    {
        public NotificationManager()
        {
        }      

        public Task SendMessageRequest(MessageRequest messageRequest)
        {
            return Task.CompletedTask;
        }
    }
}
