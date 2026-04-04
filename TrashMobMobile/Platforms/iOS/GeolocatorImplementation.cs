namespace TrashMobMobile
{
    using CommunityToolkit.Maui.Alerts;
    using CoreLocation;
    using System;
    using System.Threading.Tasks;
    using TrashMobMobile.Services;

    public class GeolocatorImplementation : IGeolocator
    {
        readonly CLLocationManager manager = new();

        public async Task StartListening(IProgress<Location> positionChangedProgress, CancellationToken cancellationToken)
        {
            var permission = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (permission != PermissionStatus.Granted)
            {
                permission = await Permissions.RequestAsync<Permissions.LocationAlways>();
                if (permission != PermissionStatus.Granted)
                {
                    await Toast.Make("No permission").Show(CancellationToken.None);
                    return;
                }
            }

            manager.AllowsBackgroundLocationUpdates = true;
            manager.PausesLocationUpdatesAutomatically = false;

            // Use NearestTenMeters accuracy instead of Best — significantly reduces battery
            // usage while still providing sufficient precision for walking-pace cleanup routes (#3263).
            manager.DesiredAccuracy = CLLocation.AccuracyNearestTenMeters;

            // 10m distance filter: eliminates redundant updates while stationary (e.g., picking
            // up litter) and matches the Android distance threshold.
            manager.DistanceFilter = 10;

            var taskCompletionSource = new TaskCompletionSource();
            cancellationToken.Register(() =>
            {
                manager.StopUpdatingLocation();
                manager.LocationsUpdated -= PositionChanged;
                taskCompletionSource.TrySetResult();
            });
            manager.LocationsUpdated += PositionChanged;
            manager.StartUpdatingLocation();

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
