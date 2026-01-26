namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Defines operations for managing event partner location services.
    /// </summary>
    public interface IEventPartnerLocationServiceManager : IBaseManager<EventPartnerLocationService>
    {
        /// <summary>
        /// Gets all current partner location services for an event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of event partner location services for the event.</returns>
        Task<IEnumerable<EventPartnerLocationService>> GetCurrentPartnersAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all potential partner locations for an event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of potential partner locations for the event.</returns>
        Task<IEnumerable<PartnerLocation>> GetPotentialPartnerLocationsAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets event partner location services by event and partner location.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="partnerLocationId">The unique identifier of the partner location.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of display event partner location services.</returns>
        Task<IEnumerable<DisplayEventPartnerLocationService>> GetByEventAndPartnerLocationAsync(Guid eventId,
            Guid partnerLocationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all partner locations for an event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of display event partner locations.</returns>
        Task<IEnumerable<DisplayEventPartnerLocation>> GetByEventAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all events associated with a partner location.
        /// </summary>
        /// <param name="partnerLocationId">The unique identifier of the partner location.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of display partner location service events.</returns>
        Task<IEnumerable<DisplayPartnerLocationServiceEvent>> GetByPartnerLocationAsync(Guid partnerLocationId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all partner location service events for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of display partner location service events for the user.</returns>
        Task<IEnumerable<DisplayPartnerLocationServiceEvent>> GetByUserAsync(Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the hauling partner location for an event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The hauling partner location for the event, or null if none exists.</returns>
        Task<PartnerLocation> GetHaulingPartnerLocationForEvent(Guid eventId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an event partner location service.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="partnerLocationId">The unique identifier of the partner location.</param>
        /// <param name="serviceTypeId">The identifier of the service type.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The number of entities deleted.</returns>
        Task<int> DeleteAsync(Guid eventId, Guid partnerLocationId, int serviceTypeId,
            CancellationToken cancellationToken = default);
    }
}
