namespace TrashMobMobileApp.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPickupLocationRestService
    {
        Task<IEnumerable<PickupLocation>> GetPickupLocationsAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<PickupLocation> GetPickupLocationAsync(Guid pickupLocationId, CancellationToken cancellationToken = default);

        Task<PickupLocation> AddPickupLocationAsync(PickupLocation pickupLocation, CancellationToken cancellationToken = default);

        Task<PickupLocation> UpdatePickupLocationAsync(PickupLocation pickupLocation, CancellationToken cancellationToken = default);

        Task SubmitLocationsAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<IEnumerable<PickupLocation>> DeletePickupLocationAsync(PickupLocation pickupLocation, CancellationToken cancellationToken = default);
    }
}