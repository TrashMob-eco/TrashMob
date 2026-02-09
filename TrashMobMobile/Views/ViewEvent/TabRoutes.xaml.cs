namespace TrashMobMobile.Views.ViewEvent;

using System.Collections.Specialized;
using Microsoft.Maui.Controls.Maps;
using TrashMob.Models.Poco;
using TrashMobMobile.ViewModels;

public partial class TabRoutes : ContentView
{
    public TabRoutes()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        if (BindingContext is ViewEventViewModel vm)
        {
            vm.EventAttendeeRoutes.CollectionChanged += OnRoutesCollectionChanged;
            RenderPolylines(vm.EventAttendeeRoutes);
        }
    }

    private void OnRoutesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (BindingContext is ViewEventViewModel vm)
        {
            RenderPolylines(vm.EventAttendeeRoutes);
        }
    }

    private void RenderPolylines(IEnumerable<DisplayEventAttendeeRoute> routes)
    {
        routeMap.MapElements.Clear();

        foreach (var route in routes)
        {
            var polyline = new Polyline
            {
                StrokeColor = Color.FromArgb("c7d762"),
                StrokeWidth = 5,
            };

            foreach (var location in route.Locations)
            {
                polyline.Geopath.Add(new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude));
            }

            routeMap.MapElements.Add(polyline);
        }
    }
}
