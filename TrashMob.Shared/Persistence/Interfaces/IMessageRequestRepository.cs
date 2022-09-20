namespace TrashMob.Shared.Persistence.Interfaces
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IMessageRequestRepository
    {
        Task<MessageRequest> AddMessageRequest(MessageRequest messageRequest);
    }
}
