namespace TrashMobMobile
{
    using CommunityToolkit.Maui.Alerts;
    using CoreLocation;
    using System;
    using System.Threading.Tasks;
    using TrashMobMobile.Services;

    public class GeolocatorImplementation : IGeolocator
    {
        private readonly CLLocationManager manager = new();

        public async Task StartListening(IProgress<Location> positionChangedProgress, CancellationToken cancellationToken)
        {
            var permission = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (permission != PermissionStatus.Granted)
            {
                permission = await Permissions.RequestAsync<Permissions.LocationAlways>();
                if (permission != PermissionStatus.Granted)
                {
                    await Toast.Make("No permission").Show(CancellationToken.None);
                    return;
                }
            }

            var taskCompletionSource = new TaskCompletionSource();
            cancellationToken.Register(() =>
            {
                manager.LocationsUpdated -= PositionChanged;
                manager.StopUpdatingLocation();
                taskCompletionSource.TrySetResult();
            });
            
            manager.PausesLocationUpdatesAutomatically = false;
            manager.AllowsBackgroundLocationUpdates = true; // ✅ Ensures updates even in background
            manager.StartUpdatingLocation(); // ✅ Ensure location tracking starts
            manager.LocationsUpdated += PositionChanged;

            void PositionChanged(object? sender, CLLocationsUpdatedEventArgs args)
            {
                if (args.Locations.Length > 0)
                {
                    var coordinate = args.Locations[^1].Coordinate;
                    positionChangedProgress.Report(new Location(coordinate.Latitude, coordinate.Longitude));
                }
            }

            await taskCompletionSource.Task;
        }
    }
}
