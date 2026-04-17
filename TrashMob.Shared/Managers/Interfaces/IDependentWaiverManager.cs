namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Info about a pending waiver request for a dependent's event registration.
    /// </summary>
    public class PendingDependentWaiverInfo
    {
        public Guid DependentId { get; set; }
        public string DependentFirstName { get; set; } = string.Empty;
        public string DependentLastName { get; set; } = string.Empty;
        public Guid EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTimeOffset EventDate { get; set; }
        public Guid MinorUserId { get; set; }
        public List<WaiverVersion> RequiredWaivers { get; set; } = [];
    }

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

        /// <summary>
        /// Gets all pending waiver requests for a parent's dependents who have waiver-pending event registrations.
        /// </summary>
        Task<IEnumerable<PendingDependentWaiverInfo>> GetPendingWaiverRequestsForParentAsync(
            Guid parentUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a dependent's waiver-pending event registrations can be resolved after a new waiver is signed.
        /// Clears WaiverPendingDate on any registrations where all required waivers are now satisfied.
        /// </summary>
        Task ResolvePendingRegistrationsAsync(
            Guid dependentId, CancellationToken cancellationToken = default);

        Task<(int Total, int Valid, int Expired, Dictionary<string, int> RelationshipBreakdown)> GetComplianceStatsAsync(CancellationToken cancellationToken = default);
    }
}
