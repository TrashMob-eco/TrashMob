namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;

public partial class AddPickupLocationViewModel : BaseViewModel
{
    [ObservableProperty]
    PickupLocationViewModel pickupLocationViewModel;

    public AddPickupLocationViewModel(IPickupLocationManager pickupLocationManager, IMapRestService mapRestService)
    {
        SavePickupLocationCommand = new Command(async () => await SavePickupLocation());
        this.pickupLocationManager = pickupLocationManager;
        this.mapRestService = mapRestService;
    }

    private Guid eventId;

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        this.eventId = eventId;

        IsBusy = false;
    }

    // This is only for the map point
    public ObservableCollection<PickupLocationViewModel> PickupLocations { get; set; } = new ObservableCollection<PickupLocationViewModel>();

    public ICommand SavePickupLocationCommand { get; set; }

    private readonly IPickupLocationManager pickupLocationManager;
    private readonly IMapRestService mapRestService;

    public async Task UpdateLocation()
    {
        Location location = await GetCurrentLocation();
        var address = await mapRestService.GetAddressAsync(location.Latitude, location.Longitude);
        PickupLocationViewModel.Address.Longitude = location.Longitude;
        PickupLocationViewModel.Address.Latitude = location.Latitude;
        PickupLocationViewModel.Address.City = address.City;
        PickupLocationViewModel.Address.Country = address.Country;
        PickupLocationViewModel.Address.County = address.County;
        PickupLocationViewModel.Address.PostalCode = address.PostalCode;
        PickupLocationViewModel.Address.Region = address.Region;
        PickupLocationViewModel.Address.StreetAddress = address.StreetAddress;
    }

    public async Task<Location> GetCurrentLocation()
    {
        try
        {
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            var cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, cancelTokenSource.Token);

            return location;
        }
        //catch (FeatureNotSupportedException fnsEx)
        //{
        //    // Handle not supported on device exception
        //}
        //catch (FeatureNotEnabledException fneEx)
        //{
        //    // Handle not enabled on device exception
        //}
        //catch (PermissionException pEx)
        //{
        //    // Handle permission exception
        //}
        catch
        {
            // Unable to get location
        }

        return null;
    }

    private async Task SavePickupLocation()
    {
        IsBusy = true;

        var pickupLocation = new PickupLocation
        {
            City = PickupLocationViewModel.Address.City,
            Country = PickupLocationViewModel.Address.Country,
            EventId = eventId,
            HasBeenPickedUp = false,
            HasBeenSubmitted = false,
            Latitude = PickupLocationViewModel.Address.Latitude,
            Longitude = PickupLocationViewModel.Address.Longitude,
            Notes = PickupLocationViewModel.Notes,
            PostalCode = PickupLocationViewModel.Address.PostalCode,
            Region = PickupLocationViewModel.Address.Region,
            StreetAddress = PickupLocationViewModel.Address.StreetAddress
        };

        await pickupLocationManager.AddPickupLocationAsync(pickupLocation);

        IsBusy = false;

        await Notify("Pickup Location has been saved.");
        await Navigation.PopAsync();
    }
}
