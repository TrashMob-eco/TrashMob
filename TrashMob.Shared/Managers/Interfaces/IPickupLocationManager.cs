namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing pickup locations.
    /// </summary>
    public interface IPickupLocationManager : IKeyedManager<PickupLocation>
    {
        /// <summary>
        /// Submits pickup locations for an event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="userId">The unique identifier of the user submitting the locations.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SubmitPickupLocationsAsync(Guid eventId, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Marks a pickup location as picked up.
        /// </summary>
        /// <param name="pickupLocationId">The unique identifier of the pickup location.</param>
        /// <param name="userId">The unique identifier of the user marking the pickup.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkAsPickedUpAsync(Guid pickupLocationId, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all pickup locations for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of pickup locations for the user.</returns>
        Task<IEnumerable<PickupLocation>> GetByUserAsync(Guid userId, CancellationToken cancellationToken);
    }
}
