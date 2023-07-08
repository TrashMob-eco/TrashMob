namespace TrashMobMobileApp.Features.Map;

using Maui.GoogleMaps;
using TrashMob.Models;
using TrashMobMobileApp.Data;

public partial class UserLocationPreferencePopup
{
    private User user;
    private const int DefaultTravelDistance = 5;
    private Map mappy;

    public IMapRestService MapRestService { get; set; }

    public IUserManager UserManager { get; set; }

    public UserLocationPreferencePopup(IUserManager userManager, IMapRestService mapRestService)
    {
        InitializeComponent();
#if !WINDOWS
        mappy = new Map();
        mappy.Loaded += mappy_Loaded;
        mappy.MapClicked += Map_MapClicked;
        mapGrid.Add(mappy);
#else
        // Add label with text to mapGrid view
        mapGrid.Add(new Label { Text = "Map not supported on Windows" });
#endif

        UserManager = userManager;
        MapRestService = mapRestService;
        units.Items.Add("miles");
        units.Items.Add("kilometers");
    }

    private void SetFields(User user)
    {
        city.Text = user.City;
        state.Text = user.Region;
        postalCode.Text = user.PostalCode;
        travelLimitForLocalEvents.Text = user.TravelLimitForLocalEvents.ToString();

        if (user.PrefersMetric)
        {
            units.SelectedItem = "kilometers";
        }
        else
        {
            units.SelectedItem = "miles";
        }
    }

    private async void mappy_Loaded(object sender, EventArgs e)
    {
        try
        {
            user = await UserManager.GetUserAsync(App.CurrentUser.Id.ToString());
        }
        catch (Exception)
        {
            // log exception somewhere
            // try user service one more time
            user = await UserManager.GetUserAsync(App.CurrentUser.Id.ToString());
        }

        var locationHelper = new LocationHelper();

        var userLocation = new Position(user.Latitude ?? 0, user.Longitude ?? 0);
        if (user.Latitude == null || user.Longitude == null || user.Latitude == 0 && user.Longitude == 0)
        {
            userLocation = await locationHelper.GetCurrentLocation();
        }

        if (user.TravelLimitForLocalEvents == 0)
        {
            user.TravelLimitForLocalEvents = DefaultTravelDistance;
        }

        SetFields(user);

        if (userLocation != null)
        {
            var mapSpan = new MapSpan(userLocation, LocationHelper.DefaultLatitudeDegreesMultipleEvents, LocationHelper.DefaultLongitudeDegreesMultipleEvents);
            mappy.MoveToRegion(mapSpan);
        }
        else
        {
            userLocation = new Position(LocationHelper.DefaultLatitude, LocationHelper.DefaultLongitude);
            var mapSpan = new MapSpan(userLocation, LocationHelper.DefaultLatitudeDegreesMultipleEvents, LocationHelper.DefaultLongitudeDegreesMultipleEvents);
            mappy.MoveToRegion(mapSpan);
        }

        mappy.Pins.Clear();
        var pin = MapHelper.GetPinForUser(user);
        pin.Position = userLocation;
        mappy.Pins.Add(pin);

        mappy.MyLocationEnabled = true;
    }

    private async void Map_MapClicked(object sender, MapClickedEventArgs e)
    {
        var map = (Map)sender;
        if (map != null)
        {
            user.Longitude = e.Point.Longitude;
            user.Latitude = e.Point.Latitude;

            // Get the actual address for this point
            var address = await MapRestService.GetAddressAsync(e.Point.Latitude, e.Point.Longitude);
            user.City = address.City;
            user.Region = address.Region;
            user.Country = address.Country;
            user.PostalCode = address.PostalCode;

            map.Pins.Clear();
            var pin = MapHelper.GetPinForUser(user);
            pin.Position = e.Point;
            map.Pins.Add(pin);

            SetFields(user);
        }
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        user.TravelLimitForLocalEvents = Convert.ToInt32(travelLimitForLocalEvents.Text);
        var unitSelected = units.SelectedItem.ToString();

        user.PrefersMetric = unitSelected != "miles";

        await UserManager.UpdateUserAsync(user);
        
        Close();
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}