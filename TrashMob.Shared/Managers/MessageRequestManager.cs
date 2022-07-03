
namespace TrashMob.Shared.Managers
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class MessageRequestManager : IMessageRequestManager
    {
        private readonly INotificationManager notificationManager;
        private readonly IMessageRequestRepository messageRequestRepository;

        public MessageRequestManager(IMessageRequestRepository messageRequestRepository, INotificationManager notificationManager)
        {
            this.messageRequestRepository = messageRequestRepository;
            this.notificationManager = notificationManager;
        }      

        public async Task SendMessageRequest(MessageRequest messageRequest)
        {
            await messageRequestRepository.AddMessageRequest(messageRequest);
            await notificationManager.SendMessageRequest(messageRequest);
        }
    }
}
