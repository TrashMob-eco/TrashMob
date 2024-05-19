namespace TrashMob.Shared.Managers.Interfaces
{    
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using TrashMob.Models;

    public interface ILitterReportManager : IKeyedManager<LitterReport>
    {
        Task<IEnumerable<LitterReport>> GetNewLitterReportsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<LitterReport>> GetAssignedLitterReportsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<LitterReport>> GetCleanedLitterReportsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<LitterReport>> GetNotCancelledLitterReportsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<LitterReport>> GetCancelledLitterReportsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<LitterReport>> GetUserLitterReportsAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<int> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    }
}