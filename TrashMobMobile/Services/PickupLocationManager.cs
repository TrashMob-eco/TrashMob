namespace TrashMobMobile.Data
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

            if (pickupLocations != null && pickupLocations.Any())
            {
                var pickupLocationImages = new List<PickupLocationImage>();

                foreach (var pickupLocation in pickupLocations)
                {
                    var url = await pickupLocationRestService.GetPickupLocationImageAsync(pickupLocation.Id, cancellationToken);

                    var pickupLocationImage = new PickupLocationImage
                    {
                        City = pickupLocation.City,
                        Country = pickupLocation.Country,
                        County = pickupLocation.County,
                        CreatedByUser = pickupLocation.CreatedByUser,
                        CreatedByUserId = pickupLocation.CreatedByUserId,
                        CreatedDate = pickupLocation.CreatedDate,
                        Event = pickupLocation.Event,
                        EventId = pickupLocation.EventId,
                        HasBeenPickedUp = pickupLocation.HasBeenPickedUp,
                        HasBeenSubmitted = pickupLocation.HasBeenSubmitted,
                        Id = pickupLocation.Id,
                        ImageUrl = string.IsNullOrEmpty(url) ? "https://www.trashmob.eco/TrashMobEco_CircleLogo.png" : url,
                        LastUpdatedByUser = pickupLocation.LastUpdatedByUser,
                        LastUpdatedByUserId = pickupLocation.LastUpdatedByUserId,
                        LastUpdatedDate = pickupLocation.LastUpdatedDate,
                        Latitude = pickupLocation.Latitude,
                        Longitude = pickupLocation.Longitude,
                        Notes = pickupLocation.Notes,
                        PostalCode = pickupLocation.PostalCode,
                        Region = pickupLocation.Region,
                        StreetAddress = pickupLocation.StreetAddress
                    };

                    pickupLocationImages.Add(pickupLocationImage);
                }

                return pickupLocationImages;
            }

            return new List<PickupLocationImage>();
        }

        public async Task<PickupLocationImage> GetPickupLocationImageAsync(Guid pickupLocationId, CancellationToken cancellationToken = default)
        {
            var pickupLocation = await pickupLocationRestService.GetPickupLocationAsync(pickupLocationId, cancellationToken);

            var url = await pickupLocationRestService.GetPickupLocationImageAsync(pickupLocation.Id, cancellationToken);

            var pickupLocationImage = new PickupLocationImage
            {
                City = pickupLocation.City,
                Country = pickupLocation.Country,
                County = pickupLocation.County,
                CreatedByUser = pickupLocation.CreatedByUser,
                CreatedByUserId = pickupLocation.CreatedByUserId,
                CreatedDate = pickupLocation.CreatedDate,
                Event = pickupLocation.Event,
                EventId = pickupLocation.EventId,
                HasBeenPickedUp = pickupLocation.HasBeenPickedUp,
                HasBeenSubmitted = pickupLocation.HasBeenSubmitted,
                Id = pickupLocation.Id,
                ImageUrl = string.IsNullOrEmpty(url) ? "https://www.trashmob.eco/TrashMobEco_CircleLogo.png" : url,
                LastUpdatedByUser = pickupLocation.LastUpdatedByUser,
                LastUpdatedByUserId = pickupLocation.LastUpdatedByUserId,
                LastUpdatedDate = pickupLocation.LastUpdatedDate,
                Latitude = pickupLocation.Latitude,
                Longitude = pickupLocation.Longitude,
                Notes = pickupLocation.Notes,
                PostalCode = pickupLocation.PostalCode,
                Region = pickupLocation.Region,
                StreetAddress = pickupLocation.StreetAddress
            };

            return pickupLocationImage;
        }

        public Task SubmitLocationsAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return pickupLocationRestService.SubmitLocationsAsync(eventId, cancellationToken);
        }

        public Task AddPickupLocationImageAsync(Guid eventId, Guid pickupLocationId, string localFileName, CancellationToken cancellationToken = default)
        {
            return pickupLocationRestService.AddPickupLocationImageAsync(eventId, pickupLocationId, localFileName, cancellationToken);
        }
    }
}
