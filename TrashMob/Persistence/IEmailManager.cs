namespace TrashMob.Persistence
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;

    public interface IEmailManager
    {
        Task SendSystemEmail(Email email, CancellationToken cancellationToken);
    }
}
