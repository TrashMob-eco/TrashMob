namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
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
    }
}
