namespace TrashMobMobileApp.Features.Map;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using TrashMob.Models;
using TrashMobMobileApp.Data;

public partial class UserLocationPreferencePopup
{
    private readonly IMapRestService mapRestService;
    private readonly IUserManager userManager;
    private User user;
    private const double DefaultLatitudeDegrees = 0.02;
    private const double DefaultLongitudeDegrees = 0.02;
    private const double DefaultLatitude = 39.8283;
    private const double DefaultLongitude = 98.5795;

    public UserLocationPreferencePopup(IMapRestService mapRestService, IUserManager userManager)
    {
        InitializeComponent();
        this.mapRestService = mapRestService;
        this.userManager = userManager;
    }

    private void SetFields(User user)
    {
        city.Text = user.City;
        state.Text = user.Region;
        postalCode.Text = user.PostalCode;
    }

    private async void mappy_Loaded(object sender, EventArgs e)
    {
        try
        {
            user = await userManager.GetUserAsync(App.CurrentUser.Id.ToString());
        }
        catch (Exception)
        {
            //log exception somewhere
            //try user service one more time
            user = await userManager.GetUserAsync(App.CurrentUser.Id.ToString());
        }

        var locationHelper = new LocationHelper();
        var userLocation = await locationHelper.GetCurrentLocation();

        if (userLocation != null)
        {
            var mapSpan = new MapSpan(userLocation, DefaultLatitudeDegrees, DefaultLongitudeDegrees);
            mappy.MoveToRegion(mapSpan);
        }
        else
        {
            var defaultLocation = new Location(DefaultLatitude, DefaultLongitude);
            var mapSpan = new MapSpan(defaultLocation, DefaultLatitudeDegrees, DefaultLongitudeDegrees);
            mappy.MoveToRegion(mapSpan);
        }

        mappy.IsShowingUser = true;
        mappy.MapClicked += Map_MapClicked;
    }

    private async void Map_MapClicked(object sender, MapClickedEventArgs e)
    {
        var map = (Map)sender;
        if (map != null)
        {
            user.Longitude = e.Location.Longitude;
            user.Latitude = e.Location.Latitude;

            // Get the actual address for this point
            var address = await mapRestService.GetAddressAsync(e.Location.Latitude, e.Location.Longitude);
            user.City = address.City;
            user.Region = address.Region;
            user.Country = address.Country;
            user.PostalCode = address.PostalCode;

            map.Pins.Clear();
            var pin = MapHelper.GetPinForUser(user);
            pin.Location = e.Location;
            map.Pins.Add(pin);

            SetFields(user);
        }
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        await userManager.UpdateUserAsync(user);
        Close();
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}