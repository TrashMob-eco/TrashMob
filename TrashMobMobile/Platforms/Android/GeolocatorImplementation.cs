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
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            context.StartForegroundService(serviceIntent);
        }
        else
        {
            context.StartService(serviceIntent);
        }

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
    // 3-second interval: at walking speed (~1.4 m/s) this yields ~4m between updates,
    // well within GPS accuracy. Reduces GPS radio wake-ups by 3x vs 1-second polling (#3263).
    private const long UpdateIntervalMs = 3000;

    // 10m distance filter: eliminates redundant updates while stationary (e.g., picking up
    // litter) and still captures smooth routes at walking pace.
    private const float MinDistanceMeters = 10;

    public Action<Location>? OnLocationChangedAction { get; set; }

    LocationManager? locationManager;

    public GeolocationContinuousListener()
    {
        locationManager = (LocationManager?)Android.App.Application.Context.GetSystemService(Android.Content.Context.LocationService);

        // Use FusedProvider on Android 12+ for battery-intelligent location (combines GPS,
        // WiFi, and cell signals). Fall back to raw GPS on older devices.
        string provider;
        if (OperatingSystem.IsAndroidVersionAtLeast(31)
            && locationManager?.IsProviderEnabled(LocationManager.FusedProvider) == true)
        {
            provider = LocationManager.FusedProvider;
        }
        else
        {
            provider = LocationManager.GpsProvider;
        }

        locationManager?.RequestLocationUpdates(provider, UpdateIntervalMs, MinDistanceMeters, this);
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
