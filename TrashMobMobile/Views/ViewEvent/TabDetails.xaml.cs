namespace TrashMobMobile.Views.ViewEvent;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

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
            if (viewModel != null)
            {
                viewModel.UpdateRoutes = UpdateRoutes;
                UpdateRoutes();
            }

            MapSpan mapSpan = new MapSpan(new Location(0, 0), 0.05, 0.05);

            if (viewModel?.EventViewModel?.Address?.Location != null)
            {
                mapSpan = new MapSpan(viewModel.EventViewModel.Address.Location, 0.05, 0.05);
            }

            eventLocationMap.InitialMapSpanAndroid = mapSpan;
            eventLocationMap.MoveToRegion(mapSpan);
        }
    }

    private void UpdateRoutes()
    {
        if (BindingContext is ViewEventViewModel viewModel)
        {
            eventLocationMap.MapElements.Clear();

            foreach (var route in viewModel.EventAttendeeRoutes)
            {
                var polyline = new Polyline
                {
                    StrokeColor = Color.FromArgb("c7d762"),
                    StrokeWidth = 5,
                };

                foreach (var location in route.Locations)
                {
                    polyline.Geopath.Add(new Location(location.Latitude, location.Longitude));
                }

                eventLocationMap.MapElements.Add(polyline);
            }
        }
    }
}