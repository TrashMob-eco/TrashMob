namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TrashMob.Models;
using TrashMobMobile.Services;

public partial class ViewPickupLocationViewModel : BaseViewModel
{
    private readonly IMobEventManager mobEventManager;
    private readonly IPickupLocationManager pickupLocationManager;

    [ObservableProperty]
    private PickupLocationViewModel pickupLocationViewModel;

    public ViewPickupLocationViewModel(IPickupLocationManager pickupLocationManager, IMobEventManager mobEventManager)
    {
        this.pickupLocationManager = pickupLocationManager;
        this.mobEventManager = mobEventManager;
    }

    public ObservableCollection<PickupLocationViewModel> PickupLocations { get; set; } = new();

    public async Task Init(Guid pickupLocationId)
    {
        IsBusy = true;

        var pickupLocation =
            await pickupLocationManager.GetPickupLocationImageAsync(pickupLocationId, ImageSizeEnum.Reduced);

        var pickupLocationViewModel = new PickupLocationViewModel(pickupLocationManager, mobEventManager)
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
            Notify = Notify,
            NotifyError = NotifyError,
            Navigation = Navigation,
            PickupLocation = pickupLocation,
            ImageUrl = pickupLocation.ImageUrl,
        };

        PickupLocationViewModel = pickupLocationViewModel;

        PickupLocations.Clear();
        PickupLocations.Add(pickupLocationViewModel);

        IsBusy = false;
    }
}