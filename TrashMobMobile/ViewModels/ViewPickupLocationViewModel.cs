namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TrashMobMobile.Data;

public partial class ViewPickupLocationViewModel : BaseViewModel
{
    private readonly IPickupLocationManager pickupLocationManager;
    private readonly IMobEventManager mobEventManager;

    public ViewPickupLocationViewModel(IPickupLocationManager pickupLocationManager, IMobEventManager mobEventManager)
    {
        this.pickupLocationManager = pickupLocationManager;
        this.mobEventManager = mobEventManager;
    }

    [ObservableProperty]
    PickupLocationViewModel pickupLocationViewModel;

    [ObservableProperty]
    double overlayOpacity;

    public ObservableCollection<PickupLocationViewModel> PickupLocations { get; set; } = new ObservableCollection<PickupLocationViewModel>();

    public async Task Init(Guid pickupLocationId)
    {
        IsBusy = true;
        OverlayOpacity = 0.25; // Workaround for: https://github.com/dotnet/maui/issues/18234

        var pickupLocation = await pickupLocationManager.GetPickupLocationImageAsync(pickupLocationId);

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
