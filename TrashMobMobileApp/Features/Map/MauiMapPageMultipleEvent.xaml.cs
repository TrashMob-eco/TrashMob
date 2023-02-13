namespace TrashMobMobileApp.Features.Map;

using Microsoft.Maui.Maps;
using TrashMob.Models;

public partial class MauiMapPageMultipleEvent : ContentPage
{
    private const double DefaultLatitudeDegrees = 1.0;
    private const double DefaultLongitudeDegrees = 1.0;

    private const double DefaultLatitude = 39.8283;
    private const double DefaultLongitude = 98.5795;

    public MauiMapPageMultipleEvent()
	{
		InitializeComponent();
	}

    public MauiMapPageMultipleEvent(IEnumerable<Event> mobEvents)
    {
        InitializeComponent();

        foreach (var mobEvent in mobEvents)
        {
            var location = new Location(mobEvent.Latitude.Value, mobEvent.Longitude.Value);

            mappy.IsShowingUser = true;

            var pin = MapHelper.GetPinForEvent(mobEvent);
            pin.Location = location;

            mappy.Pins.Add(pin);
        }
    }

    private async void mappy_Loaded(object sender, EventArgs e)
    {
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
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}