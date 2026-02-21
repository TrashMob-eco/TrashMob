namespace TrashMob.Shared.Managers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manages waiver documents and user waiver agreements.
    /// </summary>
    public class WaiverManager(IKeyedRepository<Waiver> repository)
        : KeyedManager<Waiver>(repository), IWaiverManager
    {

        /// <inheritdoc />
        public async Task<Waiver> GetByNameAsync(string waiverName, CancellationToken cancellationToken = default)
        {
            var waiverStatus = await Repository.Get(w => w.Name == waiverName).FirstOrDefaultAsync(cancellationToken);
            return waiverStatus;
        }
    }
}