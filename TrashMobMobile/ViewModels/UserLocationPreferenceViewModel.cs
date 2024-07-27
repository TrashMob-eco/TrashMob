namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Config;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class UserLocationPreferenceViewModel(IUserManager userManager, IMapRestService mapRestService) : BaseViewModel
{
    private readonly IMapRestService mapRestService = mapRestService;
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    private AddressViewModel address = new AddressViewModel();

    [ObservableProperty]
    private int travelDistance = Settings.DefaultTravelDistance;

    [ObservableProperty]
    private string units;

    public ObservableCollection<AddressViewModel> Addresses { get; set; } = [];

    public async Task Init()
    {
        IsBusy = true;

        try
        {
            Addresses.Clear();
            Address = App.CurrentUser.GetAddress();
            Addresses.Add(Address);
            TravelDistance = App.CurrentUser.TravelLimitForLocalEvents;
            Units = App.CurrentUser.PrefersMetric ? "Kilometers" : "Miles";

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotifyError("An error occurred while initializing the user location preference page.");
        }
    }

    public async Task ChangeLocation(Location location)
    {
        IsBusy = true;

        try
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

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotifyError("An error occured while updating your location. Please try again.");
        }
    }

    [RelayCommand]
    private async Task UpdateLocation()
    {
        IsBusy = true;

        try
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

            IsBusy = false;

            await Notify("User location preference has been updated.");

            await Navigation.PopToRootAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotifyError("An error occured while updating your location. Please try again.");
        }
    }
}