namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
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
        this.userManager = userManager;
        this.mapRestService = mapRestService;
        address = new AddressViewModel();
    }

    public void Init()
    {
        IsBusy = true;
        
        Addresses.Clear();
        Address = App.CurrentUser.GetAddress();
        Addresses.Add(Address);
        TravelDistance = App.CurrentUser.TravelLimitForLocalEvents;
        Units = App.CurrentUser.PrefersMetric ? "Kilometers" : "Miles";

        IsBusy = false;
    }

    public async Task ChangeLocation(Location location)
    {
        IsBusy = true;

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

        IsBusy = false;
    }

    [RelayCommand]
    private async Task UpdateLocation()
    {
        IsBusy = true;

        App.CurrentUser.City = Address.City;
        App.CurrentUser.Country = Address.Country;
        App.CurrentUser.Latitude = Address.Latitude;
        App.CurrentUser.Longitude = Address.Longitude;
        App.CurrentUser.Country = Address.Country;
        App.CurrentUser.PostalCode = Address.PostalCode;
        App.CurrentUser.TravelLimitForLocalEvents = TravelDistance;
        App.CurrentUser.PrefersMetric = Units == "Kilometers";

        await userManager.UpdateUserAsync(App.CurrentUser);

        IsBusy = false;

        await Notify("User location preference has been updated.");

        await Navigation.PopToRootAsync();
    }
}
