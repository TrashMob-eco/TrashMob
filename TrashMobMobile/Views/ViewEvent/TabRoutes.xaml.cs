namespace TrashMobMobile.Views.ViewEvent;

using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Maui.Controls.Maps;
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