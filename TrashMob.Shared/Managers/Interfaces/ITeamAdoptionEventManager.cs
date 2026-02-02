namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Defines operations for managing team adoption events (linking events to adoptions for compliance tracking).
    /// </summary>
    public interface ITeamAdoptionEventManager : IKeyedManager<TeamAdoptionEvent>
    {
        /// <summary>
        /// Links an event to a team adoption for compliance tracking.
        /// </summary>
        /// <param name="teamAdoptionId">The team adoption ID.</param>
        /// <param name="eventId">The event ID to link.</param>
        /// <param name="notes">Optional notes about why this event is linked.</param>
        /// <param name="userId">The user ID performing the link.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A service result containing the created link or an error.</returns>
        Task<ServiceResult<TeamAdoptionEvent>> LinkEventAsync(
            Guid teamAdoptionId,
            Guid eventId,
            string notes,
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Unlinks an event from a team adoption.
        /// </summary>
        /// <param name="teamAdoptionEventId">The team adoption event link ID.</param>
        /// <param name="userId">The user ID performing the unlink.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A service result indicating success or failure.</returns>
        Task<ServiceResult<bool>> UnlinkEventAsync(
            Guid teamAdoptionEventId,
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all events linked to a team adoption.
        /// </summary>
        /// <param name="teamAdoptionId">The team adoption ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of linked events.</returns>
        Task<IEnumerable<TeamAdoptionEvent>> GetByAdoptionIdAsync(
            Guid teamAdoptionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all adoptions linked to an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of linked adoptions.</returns>
        Task<IEnumerable<TeamAdoptionEvent>> GetByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an event is already linked to an adoption.
        /// </summary>
        /// <param name="teamAdoptionId">The team adoption ID.</param>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the event is already linked; otherwise, false.</returns>
        Task<bool> IsEventLinkedAsync(
            Guid teamAdoptionId,
            Guid eventId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets active adoptions for a team that can have events linked to them.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of active adoptions for the team.</returns>
        Task<IEnumerable<TeamAdoption>> GetActiveAdoptionsForTeamAsync(
            Guid teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates compliance tracking fields on the team adoption after linking/unlinking events.
        /// </summary>
        /// <param name="teamAdoptionId">The team adoption ID.</param>
        /// <param name="userId">The user ID performing the update.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateComplianceAsync(
            Guid teamAdoptionId,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
