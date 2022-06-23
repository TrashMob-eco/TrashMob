
namespace TrashMob.Shared.Managers
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class MessageRequestManager : IMessageRequestManager
    {
        private readonly IEmailManager emailManager;
        private readonly INotificationManager notificationManager;
        private readonly IMessageRequestRepository messageRequestRepository;
        private readonly IPartnerUserRepository partnerUserRepository;

        public MessageRequestManager(IEmailManager emailManager, IMessageRequestRepository messageRequestRepository, INotificationManager notificationManager)
        {
            this.emailManager = emailManager;
            this.messageRequestRepository = messageRequestRepository;
        }      

        public async Task SendMessageRequest(MessageRequest messageRequest)
        {
            await messageRequestRepository.AddMessageRequest(messageRequest);
            await notificationManager.SendMessageRequest(messageRequest);
        }
    }
}
