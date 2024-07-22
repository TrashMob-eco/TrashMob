namespace TrashMobMobile.Services
{
    using TrashMobMobile;

    public static class Geolocator
    {
        public static IGeolocator Default { get; }
#if Android
            = new TrashMobMobile.AndroidPlatform.GeolocatorImplementation();
#endif
#if IOS
        = new TrashMobMobile.iOSPlatform.GeolocatorImplementation();
#endif
    }
}