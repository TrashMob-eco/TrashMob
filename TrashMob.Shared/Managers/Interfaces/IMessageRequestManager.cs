namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IMessageRequestManager
    {
        Task SendMessageRequest(MessageRequest messageRequest);
    }
}
