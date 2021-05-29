namespace TrashMob.Persistence
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Poco;

    public interface IEmailManager
    {
        Task SendSystemEmail(Email email, CancellationToken cancellationToken);
    }
}
