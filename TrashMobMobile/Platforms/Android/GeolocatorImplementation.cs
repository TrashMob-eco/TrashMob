namespace TrashMobMobile;

using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using CommunityToolkit.Maui.Alerts;
using TrashMobMobile.Services;
using Location = Android.Locations.Location;
using Permissions = Microsoft.Maui.ApplicationModel.Permissions;

public class GeolocatorImplementation : IGeolocator
{
    GeolocationContinuousListener? locator;

    public async Task StartListening(IProgress<Microsoft.Maui.Devices.Sensors.Location> positionChangedProgress, CancellationToken cancellationToken)
    {
        var permission = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (permission != PermissionStatus.Granted)
        {
            // Show prominent disclosure before requesting location permission (Google Play policy)
            var accepted = await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                return await App.Current!.Windows[0].Page!.DisplayAlert(
                    "Location Permission Required",
                    "TrashMob needs access to your location to record your cleanup route on a map. " +
                    "Your location will be tracked while the route recording is active and a notification is shown. " +
                    "You can stop recording at any time.",
                    "Continue",
                    "Cancel");
            });

            if (!accepted)
            {
                return;
            }

            permission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (permission != PermissionStatus.Granted)
            {
                await Toast.Make("Location permission is required to record routes.").Show(CancellationToken.None);
                return;
            }
        }

        // Start foreground service so GPS continues when app is backgrounded
        var context = Android.App.Application.Context;
        var serviceIntent = new Intent(context, typeof(LocationForegroundService));
        context.StartForegroundService(serviceIntent);

        locator = new GeolocationContinuousListener();
        var taskCompletionSource = new TaskCompletionSource();
        cancellationToken.Register(() =>
        {
            locator.Dispose();
            locator = null;

            // Stop foreground service
            context.StopService(serviceIntent);

            taskCompletionSource.TrySetResult();
        });
        locator.OnLocationChangedAction = location =>
            positionChangedProgress.Report(
                new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude));
        await taskCompletionSource.Task;
    }
}

internal class GeolocationContinuousListener : Java.Lang.Object, ILocationListener
{
    public Action<Location>? OnLocationChangedAction { get; set; }

    LocationManager? locationManager;

    public GeolocationContinuousListener()
    {
        locationManager = (LocationManager?)Android.App.Application.Context.GetSystemService(Android.Content.Context.LocationService);
        // Requests location updates each second and notify if location changes more then 100 meters
        locationManager?.RequestLocationUpdates(LocationManager.GpsProvider, 1000, 100, this);
    }

    public void OnLocationChanged(Location location)
    {
        OnLocationChangedAction?.Invoke(location);
    }

    public void OnProviderDisabled(string provider)
    {
    }

    public void OnProviderEnabled(string provider)
    {
    }

    public void OnStatusChanged(string? provider, [GeneratedEnum] Availability status, Bundle? extras)
    {
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        locationManager?.RemoveUpdates(this);
        locationManager?.Dispose();
    }
}