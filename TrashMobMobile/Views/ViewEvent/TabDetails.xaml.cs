namespace TrashMobMobile.Views.ViewEvent;

using System.Collections.Specialized;
using Microsoft.Maui.Maps;
using TrashMob.Models.Poco;
using MauiLocation = Microsoft.Maui.Devices.Sensors.Location;

public partial class TabDetails : ContentView
{
	public TabDetails()
	{
		InitializeComponent();
	}

	protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is ViewEventViewModel viewModel)
        {
            MapSpan mapSpan = new MapSpan(new MauiLocation(0, 0), 0.05, 0.05);

            if (viewModel?.EventViewModel?.Address?.Location != null)
            {
                mapSpan = new MapSpan(viewModel.EventViewModel.Address.Location, 0.05, 0.05);
            }

            eventLocationMap.InitialMapSpanAndroid = mapSpan;
            eventLocationMap.MoveToRegion(mapSpan);

            // Render any routes that were already loaded
            if (viewModel?.EventAttendeeRoutes.Count > 0)
            {
                RenderRoutes(viewModel.EventAttendeeRoutes);
            }

            // Subscribe to route changes so map stays current
            viewModel!.EventAttendeeRoutes.CollectionChanged += OnRoutesChanged;
        }
    }

    private void OnRoutesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (BindingContext is ViewEventViewModel viewModel)
        {
            RenderRoutes(viewModel.EventAttendeeRoutes);
        }
    }

    public void RenderRoutes(IEnumerable<DisplayEventAttendeeRoute> routes)
    {
        eventLocationMap.MapElements.Clear();

        foreach (var route in routes)
        {
            var polyline = new Microsoft.Maui.Controls.Maps.Polyline
            {
                StrokeColor = Color.FromArgb("#c7d762"),
                StrokeWidth = 5,
            };

            foreach (var location in route.Locations)
            {
                polyline.Geopath.Add(new MauiLocation(location.Latitude, location.Longitude));
            }

            eventLocationMap.MapElements.Add(polyline);
        }
    }
}