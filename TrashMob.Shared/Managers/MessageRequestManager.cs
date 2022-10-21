
namespace TrashMob.Shared.Managers
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class MessageRequestManager : KeyedManager<MessageRequest>, IKeyedManager<MessageRequest>
    {
        private readonly INotificationManager notificationManager;

        public MessageRequestManager(IKeyedRepository<MessageRequest> messageRequestRepository, INotificationManager notificationManager) : base(messageRequestRepository)
        {
            this.notificationManager = notificationManager;
        }      

        public async Task SendMessageRequest(MessageRequest messageRequest, CancellationToken cancellationToken = default)
        {
            await Repository.AddAsync(messageRequest);
            await notificationManager.SendMessageRequestAsync(messageRequest, cancellationToken);
        }
    }
}
