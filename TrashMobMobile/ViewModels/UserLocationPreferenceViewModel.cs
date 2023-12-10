namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class UserLocationPreferenceViewModel : BaseViewModel
{
    private readonly IUserManager userManager;

    [ObservableProperty]
    AddressViewModel address;

    [ObservableProperty]
    string units;

    [ObservableProperty]
    int travelDistance;

    public UserLocationPreferenceViewModel(IUserManager userManager)
    {
        UpdateLocationCommand = new Command(async () => await UpdateLocation());
        this.userManager = userManager;
    }

    public void Init()
    {
        Address = App.CurrentUser.GetAddress();
        TravelDistance = App.CurrentUser.TravelLimitForLocalEvents;
    }

    public ICommand UpdateLocationCommand { get; set; }

    private async Task UpdateLocation()
    {
        if (Address != null)
        {
            var location = Address.Location;

            if (location != null)
            {
                Address.Longitude = location.Longitude;
                Address.Latitude = location.Latitude;
            }

            App.CurrentUser.City = Address.City;
            App.CurrentUser.Country = Address.Country;
            App.CurrentUser.Latitude = Address.Latitude;
            App.CurrentUser.Longitude = Address.Longitude;
            App.CurrentUser.Country = Address.Country;
            App.CurrentUser.PostalCode = Address.PostalCode;
            App.CurrentUser.TravelLimitForLocalEvents = TravelDistance;

            await userManager.UpdateUserAsync(App.CurrentUser);
        }
    }
}
