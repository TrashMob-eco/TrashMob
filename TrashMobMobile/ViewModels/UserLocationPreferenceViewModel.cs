namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Config;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class UserLocationPreferenceViewModel(IUserManager userManager, IMapRestService mapRestService, INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly IMapRestService mapRestService = mapRestService;
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    private AddressViewModel address = new();

    [ObservableProperty]
    private int travelDistance = Settings.DefaultTravelDistance;

    [ObservableProperty]
    private string units = string.Empty;

    public ObservableCollection<AddressViewModel> Addresses { get; set; } = [];

    public async Task Init()
    {
        IsBusy = true;

        try
        {
            Addresses.Clear();
            Address = userManager.CurrentUser.GetAddress();
            Addresses.Add(Address);
            TravelDistance = userManager.CurrentUser.TravelLimitForLocalEvents;
            Units = userManager.CurrentUser.PrefersMetric ? "Kilometers" : "Miles";

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while initializing the user location preference page.");
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
            await NotificationService.NotifyError("An error occurred while updating your location. Please try again.");
        }
    }

    [RelayCommand]
    private async Task UpdateLocation()
    {
        IsBusy = true;

        try
        {
            userManager.CurrentUser.City = Address.City;
            userManager.CurrentUser.Country = Address.Country;
            userManager.CurrentUser.Latitude = Address.Latitude;
            userManager.CurrentUser.Longitude = Address.Longitude;
            userManager.CurrentUser.Country = Address.Country;
            userManager.CurrentUser.PostalCode = Address.PostalCode;
            userManager.CurrentUser.TravelLimitForLocalEvents = TravelDistance;
            userManager.CurrentUser.PrefersMetric = Units == "Kilometers";

            await userManager.UpdateUserAsync(userManager.CurrentUser);

            IsBusy = false;

            await NotificationService.Notify("User location preference has been updated.");

            await Navigation.PopToRootAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while updating your location. Please try again.");
        }
    }
}