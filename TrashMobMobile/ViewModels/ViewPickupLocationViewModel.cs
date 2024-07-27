namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TrashMob.Models;
using TrashMobMobile.Services;

public partial class ViewPickupLocationViewModel(IPickupLocationManager pickupLocationManager, IMobEventManager mobEventManager, INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IPickupLocationManager pickupLocationManager = pickupLocationManager;

    [ObservableProperty]
    private PickupLocationViewModel pickupLocationViewModel;

    public ObservableCollection<PickupLocationViewModel> PickupLocations { get; set; } = new();

    public async Task Init(Guid pickupLocationId)
    {
        IsBusy = true;

        try
        {
            var pickupLocation =
                await pickupLocationManager.GetPickupLocationImageAsync(pickupLocationId, ImageSizeEnum.Reduced);

            var pickupLocationViewModel = new PickupLocationViewModel(pickupLocationManager, mobEventManager, NotificationService)
            {
                Address = new AddressViewModel
                {
                    City = pickupLocation.City,
                    Country = pickupLocation.Country,
                    County = pickupLocation.County,
                    Location = new Location(pickupLocation.Latitude.Value, pickupLocation.Longitude.Value),
                    Latitude = pickupLocation.Latitude.Value,
                    Longitude = pickupLocation.Longitude.Value,
                    PostalCode = pickupLocation.PostalCode,
                    Region = pickupLocation.Region,
                    StreetAddress = pickupLocation.StreetAddress,
                },
                Id = pickupLocation.Id,
                Notes = pickupLocation.Notes,
                Name = pickupLocation.Name,
                Navigation = Navigation,
                PickupLocation = pickupLocation,
                ImageUrl = pickupLocation.ImageUrl,
            };

            PickupLocationViewModel = pickupLocationViewModel;

            PickupLocations.Clear();
            PickupLocations.Add(pickupLocationViewModel);

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while loading the pickup location. Please try again.");
        }
    }
}