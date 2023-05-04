namespace TrashMobMobileApp.Features.Map;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using TrashMob.Models;
using TrashMobMobileApp.Data;

public partial class EditMapPopup
{
    public IMapRestService MapRestService { get; set; }

    private readonly Event mobEvent;
    private const double DefaultLatitudeDegrees = 0.02;
    private const double DefaultLongitudeDegrees = 0.02;
    private const double DefaultLatitude = 39.8283;
    private const double DefaultLongitude = 98.5795;

    public EditMapPopup(IMapRestService mapRestService, Event mobEvent)
    {
        InitializeComponent();
        MapRestService = mapRestService;
        this.mobEvent = mobEvent;
    }

    private void SetFields(Event mobEvent)
    {
        streetAddress.Text = mobEvent.StreetAddress;
        city.Text = mobEvent.City;
        state.Text = mobEvent.Region;
        postalCode.Text = mobEvent.PostalCode;
    }

    private async void mappy_Loaded(object sender, EventArgs e)
    {
        var locationHelper = new LocationHelper();

        if (mobEvent.Latitude != null && mobEvent.Longitude != null)
        {
            var eventLocation = new Location(mobEvent.Latitude.Value, mobEvent.Longitude.Value);
            var mapSpan = new MapSpan(eventLocation, DefaultLatitudeDegrees, DefaultLongitudeDegrees);
            mappy.MoveToRegion(mapSpan);

            mappy.Pins.Clear();
            var pin = MapHelper.GetPinForEvent(mobEvent);
            pin.Location = eventLocation;
            mappy.Pins.Add(pin);
            SetFields(mobEvent);
        }
        else
        {
            mappy.Pins.Clear();
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
        }

        NextButton.IsEnabled = false;
        mappy.IsShowingUser = true;
        mappy.MapClicked += Map_MapClicked;
    }

    private async void Map_MapClicked(object sender, MapClickedEventArgs e)
    {
        var map = (Map)sender;
        if (map != null)
        {
            mobEvent.Longitude = e.Location.Longitude;
            mobEvent.Latitude = e.Location.Latitude;

            // Get the actual address for this point
            var address = await MapRestService.GetAddressAsync(e.Location.Latitude, e.Location.Longitude);
            mobEvent.StreetAddress = address.StreetAddress;
            mobEvent.City = address.City;
            mobEvent.Region = address.Region;
            mobEvent.Country = address.Country;
            mobEvent.PostalCode = address.PostalCode;

            map.Pins.Clear();
            var pin = MapHelper.GetPinForEvent(mobEvent);
            pin.Location = e.Location;
            map.Pins.Add(pin);

            SetFields(mobEvent);
            NextButton.IsEnabled = true;
        }
    }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    private void NextButton_Clicked(object sender, EventArgs e)
    {
        Close(mobEvent);
    }
}