namespace TrashMob.Shared.Managers
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface INotificationManager
    {
        Task SendMessageRequest(MessageRequest messageRequest);
    }
}
