namespace TrashMobMobileApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public class PickupLocationManager : IPickupLocationManager
    {
        private readonly IPickupLocationRestService pickupLocationRestService;

        public PickupLocationManager(IPickupLocationRestService pickupLocationRestService)
        {
            this.pickupLocationRestService = pickupLocationRestService;
        }

        public Task<PickupLocation> GetPickupLocationAsync(Guid pickupLocationId, CancellationToken cancellationToken = default)
        {
            return pickupLocationRestService.GetPickupLocationAsync(pickupLocationId, cancellationToken);
        }

        public Task<PickupLocation> UpdatePickupLocationAsync(PickupLocation pickupLocation, CancellationToken cancellationToken = default)
        {
            return pickupLocationRestService.UpdatePickupLocationAsync(pickupLocation, cancellationToken);
        }

        public Task<PickupLocation> AddPickupLocationAsync(PickupLocation pickupLocation, CancellationToken cancellationToken = default)
        {
            return pickupLocationRestService.AddPickupLocationAsync(pickupLocation, cancellationToken);
        }

        public Task<IEnumerable<PickupLocation>> DeletePickupLocationAsync(PickupLocation pickupLocation, CancellationToken cancellationToken = default)
        {
            return pickupLocationRestService.DeletePickupLocationAsync(pickupLocation, cancellationToken);
        }

        public async Task<IEnumerable<PickupLocationImage>> GetPickupLocationsAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var pickupLocations = await pickupLocationRestService.GetPickupLocationsAsync(eventId, cancellationToken);
            var pickupLocationImages = new List<PickupLocationImage>(pickupLocations as IEnumerable<PickupLocationImage>);

            foreach (var pickupLocationImage in pickupLocationImages)
            {
                var url = await pickupLocationRestService.GetPickupLocationImageAsync(pickupLocationImage.Id, cancellationToken);
                pickupLocationImage.ImageUrl = string.IsNullOrEmpty(url) ? "https://www.trashmob.eco/TrashMobEco_CircleLogo.png" : url;
            }

            return pickupLocationImages;
        }

        public Task SubmitLocationsAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return pickupLocationRestService.SubmitLocationsAsync(eventId, cancellationToken);
        }

        public Task AddPickupLocationImageAsync(Guid eventId, Guid pickupLocationId, string localFileName, CancellationToken cancellationToken = default)
        {
            return pickupLocationRestService.AddPickupLocationImageAsync(eventId, pickupLocationId, localFileName, cancellationToken);
        }

        public async Task<string> GetPickupLocationImageAsync(Guid pickupLocationId, CancellationToken cancellationToken = default)
        {
            return await pickupLocationRestService.GetPickupLocationImageAsync(pickupLocationId, cancellationToken);
        }
    }
}
