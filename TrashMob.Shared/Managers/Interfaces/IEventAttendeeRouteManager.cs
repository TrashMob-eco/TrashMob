namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Defines operations for managing event attendee routes.
    /// </summary>
    public interface IEventAttendeeRouteManager : IBaseManager<EventAttendeeRoute>
    {
        /// <summary>
        /// Gets anonymized (no UserId) non-private, non-expired routes for an event.
        /// </summary>
        Task<IEnumerable<DisplayAnonymizedRoute>> GetAnonymizedRoutesForEventAsync(Guid eventId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets aggregated route statistics for an event.
        /// </summary>
        Task<DisplayEventRouteStats> GetEventRouteStatsAsync(Guid eventId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a user's route history with event context.
        /// </summary>
        Task<IEnumerable<DisplayUserRouteHistory>> GetUserRouteHistoryAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates route metadata (privacy, trim, notes). Only the route owner can update.
        /// </summary>
        Task<ServiceResult<EventAttendeeRoute>> UpdateRouteMetadataAsync(Guid routeId, Guid userId, UpdateRouteMetadataRequest request, CancellationToken cancellationToken = default);
    }
}
