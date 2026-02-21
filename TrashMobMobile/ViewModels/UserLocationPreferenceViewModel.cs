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
        await ExecuteAsync(async () =>
        {
            Addresses.Clear();
            Address = userManager.CurrentUser.GetAddress();
            Addresses.Add(Address);
            TravelDistance = userManager.CurrentUser.TravelLimitForLocalEvents;
            Units = userManager.CurrentUser.PrefersMetric ? "Kilometers" : "Miles";
        }, "An error occurred while initializing the user location preference page.");
    }

    public async Task ChangeLocation(Location location)
    {
        await ExecuteAsync(async () =>
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
        }, "An error occurred while updating your location. Please try again.");
    }

    [RelayCommand]
    private async Task UpdateLocation()
    {
        await ExecuteAsync(async () =>
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

            await NotificationService.Notify("User location preference has been updated.");

            await Navigation.PopToRootAsync();
        }, "An error occurred while updating your location. Please try again.");
    }
}