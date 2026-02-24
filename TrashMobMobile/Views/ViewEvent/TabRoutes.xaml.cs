namespace TrashMobMobile.Views.ViewEvent;

using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using TrashMob.Models.Poco;
using TrashMobMobile.ViewModels;

public partial class TabRoutes : ContentView
{
    public TabRoutes()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (BindingContext is ViewEventViewModel vm)
        {
            vm.EventAttendeeRoutes.CollectionChanged += OnRoutesCollectionChanged;
            vm.PropertyChanged += OnViewModelPropertyChanged;

            if (vm.AreRoutesFound)
            {
                RenderPolylines(vm.EventAttendeeRoutes);
            }
        }
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        if (BindingContext is ViewEventViewModel vm)
        {
            vm.EventAttendeeRoutes.CollectionChanged -= OnRoutesCollectionChanged;
            vm.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }

    private void OnRoutesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (BindingContext is ViewEventViewModel vm && routeMap.IsVisible)
        {
            // Only render when the map is already visible. When the first route is added,
            // the map is still hidden (AreRoutesFound hasn't been set yet). Rendering
            // polylines on a hidden Android MapView crashes the native Google Maps SDK.
            // The AreRoutesFound property change handler below covers that case.
            RenderPolylines(vm.EventAttendeeRoutes);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewEventViewModel.AreRoutesFound)
            && BindingContext is ViewEventViewModel vm
            && vm.AreRoutesFound)
        {
            // Map just became visible — render any routes that were added while it was hidden.
            RenderPolylines(vm.EventAttendeeRoutes);
        }
    }

    private void RenderPolylines(IEnumerable<DisplayEventAttendeeRoute> routes)
    {
        routeMap.MapElements.Clear();

        var allPoints = new List<Microsoft.Maui.Devices.Sensors.Location>();

        foreach (var route in routes)
        {
            var colorHex = route.DensityColor?.TrimStart('#') ?? "9E9E9E";
            var polyline = new Polyline
            {
                StrokeColor = Color.FromArgb(colorHex),
                StrokeWidth = 5,
            };

            foreach (var location in route.Locations)
            {
                var point = new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude);
                polyline.Geopath.Add(point);
                allPoints.Add(point);
            }

            routeMap.MapElements.Add(polyline);
        }

        if (allPoints.Count > 0)
        {
            var minLat = allPoints.Min(p => p.Latitude);
            var maxLat = allPoints.Max(p => p.Latitude);
            var minLon = allPoints.Min(p => p.Longitude);
            var maxLon = allPoints.Max(p => p.Longitude);

            var centerLat = (minLat + maxLat) / 2;
            var centerLon = (minLon + maxLon) / 2;

            // Add padding so the route doesn't touch the map edges
            var latDelta = Math.Max((maxLat - minLat) * 1.3, 0.002);
            var lonDelta = Math.Max((maxLon - minLon) * 1.3, 0.002);

            var center = new Microsoft.Maui.Devices.Sensors.Location(centerLat, centerLon);
            var mapSpan = new MapSpan(center, latDelta, lonDelta);
            routeMap.InitialMapSpanAndroid = mapSpan;
            routeMap.MoveToRegion(mapSpan);
        }
    }
}