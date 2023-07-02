namespace TrashMobMobileApp.Features.Map;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using TrashMob.Models;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Extensions;

public partial class MauiMapPageSingleEvent : ContentPage
{
    private readonly Event mobEvent;
    private const double DefaultLatitudeDegrees = 0.02;
    private const double DefaultLongitudeDegrees = 0.02;
    private const double DefaultLatitude = 39.8283;
    private const double DefaultLongitude = 98.5795;

    public IMapRestService MapRestService { get; set; }

    public MauiMapPageSingleEvent()
	{
		InitializeComponent();
	}

    public MauiMapPageSingleEvent(IMapRestService mapRestService, Event mobEvent, bool isViewOnly = true)
    {
        InitializeComponent();
        MapRestService = mapRestService;
        this.mobEvent = mobEvent;

        var location = new Location(mobEvent.Latitude.Value, mobEvent.Longitude.Value);

        if (location != null)
        {
            var mapSpan = new MapSpan(location, DefaultLatitudeDegrees, DefaultLongitudeDegrees);
            mappy.MoveToRegion(mapSpan);
        }
        else
        {
            var defaultLocation = new Location(DefaultLatitude, DefaultLongitude);
            var mapSpan = new MapSpan(defaultLocation, DefaultLatitudeDegrees, DefaultLongitudeDegrees);
            mappy.MoveToRegion(mapSpan);
        }

        mappy.IsShowingUser = true;

        var pin = MapHelper.GetPinForEvent(mobEvent);
        pin.Location = location;

        SetFields(mobEvent);

        mappy.Pins.Add(pin);

        if (mobEvent.IsEventLead() && !isViewOnly)
        {
            mappy.MapClicked += Map_MapClicked;
        }
    }

    private void SetFields(Event mobEvent)
    {
        eventName.Text = mobEvent.Name;
        eventDate.Text = mobEvent.EventDate.GetFormattedLocalDate();
        eventTime.Text = mobEvent.EventDate.GetFormattedLocalTime();
        streetAddress.Text = mobEvent.StreetAddress;
        city.Text = mobEvent.City;
        state.Text = mobEvent.Region;
        postalCode.Text = mobEvent.PostalCode;
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
        }
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}