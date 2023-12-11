namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class UserLocationPreferenceViewModel : BaseViewModel
{
    private readonly IUserManager userManager;
    private readonly IMapRestService mapRestService;

    public ObservableCollection<AddressViewModel> Addresses { get; set; } = [];

    [ObservableProperty]
    string units;

    [ObservableProperty]
    int travelDistance;

    [ObservableProperty]
    AddressViewModel address;

    public UserLocationPreferenceViewModel(IUserManager userManager, IMapRestService mapRestService)
    {
        UpdateLocationCommand = new Command(async () => await UpdateLocation());
        this.userManager = userManager;
        this.mapRestService = mapRestService;
        address = new AddressViewModel();
    }

    public void Init()
    {
        Addresses.Clear();
        Address = App.CurrentUser.GetAddress();
        Addresses.Add(Address);
        TravelDistance = App.CurrentUser.TravelLimitForLocalEvents;
        Units = App.CurrentUser.PrefersMetric ? "Kilometers" : "Miles";
    }

    public ICommand UpdateLocationCommand { get; set; }

    public async Task ChangeLocation(Location location)
    {
        var addr = await mapRestService.GetAddressAsync(location.Latitude, location.Longitude);

        Address.City = addr.City;
        Address.Country = addr.Country;
        Address.Latitude = location.Latitude;
        Address.Longitude = location.Longitude;
        Address.Location = location;
        Address.PostalCode = addr.PostalCode;
        Address.Region = addr.Region;
        Address.StreetAddress = addr.StreetAddress;

        Addresses.Clear();
        Addresses.Add(Address);
    }

    private async Task UpdateLocation()
    {
        App.CurrentUser.City = Address.City;
        App.CurrentUser.Country = Address.Country;
        App.CurrentUser.Latitude = Address.Latitude;
        App.CurrentUser.Longitude = Address.Longitude;
        App.CurrentUser.Country = Address.Country;
        App.CurrentUser.PostalCode = Address.PostalCode;
        App.CurrentUser.TravelLimitForLocalEvents = TravelDistance;
        App.CurrentUser.PrefersMetric = Units == "Kilometers";

        await userManager.UpdateUserAsync(App.CurrentUser);
    }
}
