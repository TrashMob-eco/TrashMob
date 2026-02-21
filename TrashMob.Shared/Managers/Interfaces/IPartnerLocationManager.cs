namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing partner locations.
    /// </summary>
    public interface IPartnerLocationManager : IKeyedManager<PartnerLocation>
    {
        /// <summary>
        /// Gets the partner associated with a specific location.
        /// </summary>
        /// <param name="partnerLocationId">The unique identifier of the partner location.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The partner associated with the location.</returns>
        Task<Partner> GetPartnerForLocationAsync(Guid partnerLocationId, CancellationToken cancellationToken);

        /// <summary>
        /// Finds partners with active locations within a specified radius of a point.
        /// </summary>
        /// <param name="latitude">The latitude of the search center point.</param>
        /// <param name="longitude">The longitude of the search center point.</param>
        /// <param name="radiusMiles">The search radius in miles.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of partners with locations within the radius.</returns>
        Task<IEnumerable<Partner>> GetNearbyPartnersAsync(double latitude, double longitude, double radiusMiles, CancellationToken cancellationToken);
    }
}
