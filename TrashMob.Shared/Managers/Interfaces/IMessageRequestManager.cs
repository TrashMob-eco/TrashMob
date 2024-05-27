namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IMessageRequestManager
    {
        Task SendMessageRequestAsync(MessageRequest messageRequest);
    }
}