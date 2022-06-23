namespace TrashMob.Shared.Managers
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IMessageRequestManager
    {
        Task SendMessageRequest(MessageRequest messageRequest);
    }
}
