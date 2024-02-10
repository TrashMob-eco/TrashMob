namespace TrashMobMobileApp.Features.Map;

using Maui.GoogleMaps;
using TrashMob.Models;
using TrashMobMobileApp.Data;

public partial class EditMapPopup
{
    public IMapRestService MapRestService { get; set; }

    private readonly Event mobEvent;

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

    private async void Popup_Opened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        var locationHelper = new LocationHelper();

        if (mobEvent.Latitude != null && mobEvent.Longitude != null)
        {
            var eventLocation = new Position(mobEvent.Latitude.Value, mobEvent.Longitude.Value);
            var mapSpan = new MapSpan(eventLocation, LocationHelper.DefaultLatitudeDegreesSingleEvent, LocationHelper.DefaultLongitudeDegreesSingleEvent);
            mappy.MoveToRegion(mapSpan);

            mappy.Pins.Clear();
            var pin = MapHelper.GetPinForEvent(mobEvent);
            pin.Position = eventLocation;
            mappy.Pins.Add(pin);
            SetFields(mobEvent);
        }
        else
        {
            mappy.Pins.Clear();
            var userLocation = await locationHelper.GetCurrentLocation();

            if (userLocation != null)
            {
                var mapSpan = new MapSpan(userLocation, LocationHelper.DefaultLatitudeDegreesSingleEvent, LocationHelper.DefaultLongitudeDegreesSingleEvent);
                mappy.MoveToRegion(mapSpan);
            }
            else
            {
                var defaultLocation = new Position(LocationHelper.DefaultLatitude, LocationHelper.DefaultLongitude);
                var mapSpan = new MapSpan(defaultLocation, LocationHelper.DefaultLatitudeDegreesSingleEvent, LocationHelper.DefaultLongitudeDegreesSingleEvent);
                mappy.MoveToRegion(mapSpan);
            }
        }

        NextButton.IsEnabled = false;
        mappy.MyLocationEnabled = true;
    }

    private async void Map_MapClicked(object sender, MapClickedEventArgs e)
    {
        var map = (Map)sender;
        if (map != null)
        {
            mobEvent.Longitude = e.Point.Longitude;
            mobEvent.Latitude = e.Point.Latitude;

            // Get the actual address for this point
            var address = await MapRestService.GetAddressAsync(e.Point.Latitude, e.Point.Longitude);
            mobEvent.StreetAddress = address.StreetAddress;
            mobEvent.City = address.City;
            mobEvent.Region = address.Region;
            mobEvent.Country = address.Country;
            mobEvent.PostalCode = address.PostalCode;

            map.Pins.Clear();
            var pin = MapHelper.GetPinForEvent(mobEvent);
            pin.Position = e.Point;
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