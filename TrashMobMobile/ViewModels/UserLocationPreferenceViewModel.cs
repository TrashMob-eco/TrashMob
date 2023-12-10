namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class UserLocationPreferenceViewModel : BaseViewModel
{
    private readonly IUserManager userManager;

    public ObservableCollection<AddressViewModel> Addresses { get; set; } = [];

    [ObservableProperty]
    string units;

    [ObservableProperty]
    int travelDistance;

    public UserLocationPreferenceViewModel(IUserManager userManager)
    {
        UpdateLocationCommand = new Command(async () => await UpdateLocation());
        ClickNewLocationCommand = new Command(async () => await CaptureLocation());
        this.userManager = userManager;
    }

    public void Init()
    {
        Addresses.Clear();
        var address = App.CurrentUser.GetAddress();
        Addresses.Add(address);
        TravelDistance = App.CurrentUser.TravelLimitForLocalEvents;
    }

    public ICommand UpdateLocationCommand { get; set; }

    public ICommand ClickNewLocationCommand { get; set; }

    private async Task CaptureLocation()
    {

    }

    private async Task UpdateLocation()
    {
        var address = Addresses[0];
        if (address != null)
        {
            var location = address.Location;

            if (location != null)
            {
                address.Longitude = location.Longitude;
                address.Latitude = location.Latitude;
            }

            App.CurrentUser.City = address.City;
            App.CurrentUser.Country = address.Country;
            App.CurrentUser.Latitude = address.Latitude;
            App.CurrentUser.Longitude = address.Longitude;
            App.CurrentUser.Country = address.Country;
            App.CurrentUser.PostalCode = address.PostalCode;
            App.CurrentUser.TravelLimitForLocalEvents = TravelDistance;

            await userManager.UpdateUserAsync(App.CurrentUser);
        }
    }
}
