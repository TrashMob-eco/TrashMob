namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class CreatePickupLocationViewModel : BaseViewModel
{
    [ObservableProperty]
    PickupLocationViewModel pickupLocationViewModel;

    [ObservableProperty]
    EventViewModel eventViewModel;

    public CreatePickupLocationViewModel(IPickupLocationManager pickupLocationManager, IMapRestService mapRestService, IMobEventManager mobEventManager)
    {
        SavePickupLocationCommand = new Command(async () => await SavePickupLocation());
        this.pickupLocationManager = pickupLocationManager;
        this.mapRestService = mapRestService;
        this.mobEventManager = mobEventManager;
    }

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        var mobEvent = await mobEventManager.GetEventAsync(eventId);

        EventViewModel = mobEvent.ToEventViewModel();

        PickupLocationViewModel = new PickupLocationViewModel(pickupLocationManager, mobEventManager)
        {
            Name = "Pickup",
            Address = new AddressViewModel(),
            Notify = Notify,
            NotifyError = NotifyError,
            Navigation = Navigation,
        };

        await PickupLocationViewModel.Init(eventId);

        IsBusy = false;
    }

    // This is only for the map point
    public ObservableCollection<PickupLocationViewModel> PickupLocations { get; set; } = new ObservableCollection<PickupLocationViewModel>();

    public ICommand SavePickupLocationCommand { get; set; }

    private readonly IPickupLocationManager pickupLocationManager;
    private readonly IMapRestService mapRestService;
    private readonly IMobEventManager mobEventManager;

    public string LocalFilePath { get; set; }

    public async Task UpdateLocation()
    {
        Location? location = await GetCurrentLocation();

        if (location != null)
        {
            var address = await mapRestService.GetAddressAsync(location.Latitude, location.Longitude);
            PickupLocationViewModel.Address.Longitude = location.Longitude;
            PickupLocationViewModel.Address.Latitude = location.Latitude;
            PickupLocationViewModel.Address.City = address.City;
            PickupLocationViewModel.Address.Country = address.Country;
            PickupLocationViewModel.Address.County = address.County;
            PickupLocationViewModel.Address.PostalCode = address.PostalCode;
            PickupLocationViewModel.Address.Region = address.Region;
            PickupLocationViewModel.Address.StreetAddress = address.StreetAddress;
            PickupLocationViewModel.Address.Location = new Location(PickupLocationViewModel.Address.Latitude.Value, PickupLocationViewModel.Address.Longitude.Value);

            PickupLocations.Clear();
            PickupLocations.Add(PickupLocationViewModel);
        }
        else
        {
            await NotifyError("Could not get location for image");
        }
    }

    public static async Task<Location?> GetCurrentLocation()
    {
        try
        {
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

            var cancelTokenSource = new CancellationTokenSource();

            return await Geolocation.Default.GetLocationAsync(request, cancelTokenSource.Token);
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
            EventId = EventViewModel.Id,
            HasBeenPickedUp = false,
            HasBeenSubmitted = false,
            Latitude = PickupLocationViewModel.Address.Latitude,
            Longitude = PickupLocationViewModel.Address.Longitude,
            Notes = PickupLocationViewModel.Notes,
            Name = PickupLocationViewModel.Name,
            PostalCode = PickupLocationViewModel.Address.PostalCode,
            Region = PickupLocationViewModel.Address.Region,
            StreetAddress = PickupLocationViewModel.Address.StreetAddress,
            County = PickupLocationViewModel.Address.County,            
        };

        var updatedPickupLocation = await pickupLocationManager.AddPickupLocationAsync(pickupLocation);
        await pickupLocationManager.AddPickupLocationImageAsync(updatedPickupLocation.EventId, updatedPickupLocation.Id, LocalFilePath);

        IsBusy = false;

        await Notify("Pickup Location has been saved.");
        await Navigation.PopAsync();
    }
}
