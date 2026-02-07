namespace TrashMobMobile;

using TrashMobMobile.Services;

// Stub implementation so the Geolocator static class compiles in tests.
internal class GeolocatorImplementation : IGeolocator
{
    public Task StartListening(IProgress<Location> positionChangedProgress, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
