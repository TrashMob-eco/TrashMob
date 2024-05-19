namespace TrashMobMobile.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPickupLocationManager
    {
        Task<IEnumerable<PickupLocationImage>> GetPickupLocationsAsync(Guid eventId, ImageSizeEnum imageSize, CancellationToken cancellationToken = default);

        Task<PickupLocationImage> GetPickupLocationImageAsync(Guid pickupLocatioImageId, ImageSizeEnum imageSize, CancellationToken cancellationToken = default);

        Task<PickupLocation> GetPickupLocationAsync(Guid pickupLocationId, CancellationToken cancellationToken = default);

        Task<PickupLocation> AddPickupLocationAsync(PickupLocation pickupLocation, CancellationToken cancellationToken = default);

        Task<PickupLocation> UpdatePickupLocationAsync(PickupLocation pickupLocation, CancellationToken cancellationToken = default);

        Task SubmitLocationsAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<IEnumerable<PickupLocation>> DeletePickupLocationAsync(PickupLocation pickupLocation, CancellationToken cancellationToken = default);

        Task AddPickupLocationImageAsync(Guid eventId, Guid pickupLocationId, string localFileName, CancellationToken cancellationToken = default);
    }
}