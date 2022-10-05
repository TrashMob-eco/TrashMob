namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface INotificationManager
    {
        Task SendMessageRequestAsync(MessageRequest messageRequest, CancellationToken cancellationToken);
    }
}
