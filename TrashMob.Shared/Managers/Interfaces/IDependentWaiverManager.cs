namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    public interface IDependentWaiverManager : IKeyedManager<DependentWaiver>
    {
        Task<ServiceResult<DependentWaiver>> SignWaiverAsync(
            Guid dependentId,
            Guid waiverVersionId,
            string typedLegalName,
            string ipAddress,
            string userAgent,
            Guid signerUserId,
            CancellationToken cancellationToken = default);

        Task<bool> HasValidWaiverAsync(Guid dependentId, CancellationToken cancellationToken = default);

        Task<DependentWaiver> GetCurrentWaiverAsync(Guid dependentId, CancellationToken cancellationToken = default);

        Task<IEnumerable<DependentWaiver>> GetByDependentIdAsync(Guid dependentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the required waivers for a dependent to attend an event (Global + Community waivers not yet signed).
        /// </summary>
        Task<IEnumerable<WaiverVersion>> GetRequiredWaiversForDependentEventAsync(
            Guid dependentId, Guid eventId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a dependent has all required waivers for an event.
        /// </summary>
        Task<bool> HasValidWaiversForEventAsync(
            Guid dependentId, Guid eventId, CancellationToken cancellationToken = default);

        Task<(int Total, int Valid, int Expired, Dictionary<string, int> RelationshipBreakdown)> GetComplianceStatsAsync(CancellationToken cancellationToken = default);
    }
}
