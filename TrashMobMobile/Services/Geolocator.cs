namespace TrashMobMobile.Services
{
    using TrashMobMobile;

    public static class Geolocator
    {
        public static IGeolocator Default { get; } = new GeolocatorImplementation();
    }
}
