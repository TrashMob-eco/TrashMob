namespace TrashMobMobile.Services
{
    using TrashMob.Models;

    public interface IPickupLocationRestService
    {
        Task<IEnumerable<PickupLocation>> GetPickupLocationsAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        Task<PickupLocation> GetPickupLocationAsync(Guid pickupLocationId,
            CancellationToken cancellationToken = default);

        Task<PickupLocation> AddPickupLocationAsync(PickupLocation pickupLocation,
            CancellationToken cancellationToken = default);

        Task<PickupLocation> UpdatePickupLocationAsync(PickupLocation pickupLocation,
            CancellationToken cancellationToken = default);

        Task SubmitLocationsAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<IEnumerable<PickupLocation>> DeletePickupLocationAsync(PickupLocation pickupLocation,
            CancellationToken cancellationToken = default);

        Task AddPickupLocationImageAsync(Guid eventId, Guid pickupLocationId, string localFileName,
            CancellationToken cancellationToken = default);

        Task<string> GetPickupLocationImageAsync(Guid pickupLocationId, ImageSizeEnum imageSize,
            CancellationToken cancellationToken = default);
    }
}