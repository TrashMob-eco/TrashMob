namespace TrashMob.Shared.Managers.Contacts
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public interface IGrantManager : IKeyedManager<Grant>
    {
        Task<IEnumerable<Grant>> GetByStatusAsync(int status, CancellationToken cancellationToken = default);
    }
}
