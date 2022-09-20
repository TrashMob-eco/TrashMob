namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface INotificationManager
    {
        Task SendMessageRequest(MessageRequest messageRequest);
    }
}
