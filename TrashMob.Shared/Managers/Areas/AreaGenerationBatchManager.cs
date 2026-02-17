namespace TrashMob.Shared.Managers.Areas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manager for area generation batch operations.
    /// </summary>
    public class AreaGenerationBatchManager(IKeyedRepository<AreaGenerationBatch> repository)
        : KeyedManager<AreaGenerationBatch>(repository), IAreaGenerationBatchManager
    {
        private static readonly string[] TerminalStatuses = ["Complete", "Failed", "Cancelled"];

        /// <inheritdoc />
        public async Task<IEnumerable<AreaGenerationBatch>> GetByPartnerAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(b => b.PartnerId == partnerId)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<AreaGenerationBatch> GetActiveByPartnerAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(b => b.PartnerId == partnerId && !TerminalStatuses.Contains(b.Status))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
