namespace TrashMobMobileApp.Features.Map;

using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using TrashMob.Models;
using TrashMobMobileApp.Data;

public partial class UserLocationPreferencePopup
{
    private User user;
    private const double DefaultLatitudeDegrees = 1.00;
    private const double DefaultLongitudeDegrees = 1.00;
    private const double DefaultLatitude = 39.8283;
    private const double DefaultLongitude = 98.5795;
    private const int DefaultTravelDistance = 5;

    public IMapRestService MapRestService { get; set; }

    public IUserManager UserManager { get; set; }

    public UserLocationPreferencePopup(IUserManager userManager, IMapRestService mapRestService)
    {
        InitializeComponent();
        travelLimitForLocalEvents = new Slider(1, 100, 5);
        UserManager = userManager;
        MapRestService = mapRestService;
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
            user = await UserManager.GetUserAsync(App.CurrentUser.Id.ToString());
        }
        catch (Exception)
        {
            //log exception somewhere
            //try user service one more time
            user = await UserManager.GetUserAsync(App.CurrentUser.Id.ToString());
        }

        var locationHelper = new LocationHelper();

        Location userLocation = new Location(user.Latitude.Value, user.Longitude.Value);
        if (user.Latitude == null || user.Longitude == null || user.Latitude == 0 && user.Longitude == 0)
        {
            userLocation = await locationHelper.GetCurrentLocation();
        }

        if (user.TravelLimitForLocalEvents == 0)
        {
            user.TravelLimitForLocalEvents = DefaultTravelDistance;
        }

        travelLimitForLocalEvents.Value = user.TravelLimitForLocalEvents;
        prefersMetrics.IsChecked = user.PrefersMetric;

        if (userLocation != null)
        {
            var mapSpan = new MapSpan(userLocation, DefaultLatitudeDegrees, DefaultLongitudeDegrees);
            mappy.MoveToRegion(mapSpan);
        }
        else
        {
            userLocation = new Location(DefaultLatitude, DefaultLongitude);
            var mapSpan = new MapSpan(userLocation, DefaultLatitudeDegrees, DefaultLongitudeDegrees);
            mappy.MoveToRegion(mapSpan);
        }

        mappy.Pins.Clear();
        var pin = MapHelper.GetPinForUser(user);
                pin.Location = userLocation;
        mappy.Pins.Add(pin);

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
            var address = await MapRestService.GetAddressAsync(e.Location.Latitude, e.Location.Longitude);
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
        user.TravelLimitForLocalEvents = (int)travelLimitForLocalEvents.Value;
        user.PrefersMetric = prefersMetrics.IsChecked;

        await UserManager.UpdateUserAsync(user);
        
        Close();
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}