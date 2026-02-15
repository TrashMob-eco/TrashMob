namespace TrashMobMobile.Config;

public static class Settings
{
#if USETEST
    public const string ApiBaseUrl = "https://dev.trashmob.eco/api/";

    public const string SiteBaseUrl = "https://dev.trashmob.eco";
#else
    public const string ApiBaseUrl = "https://www.trashmob.eco/api/";

    public const string SiteBaseUrl = "https://www.trashmob.eco";
#endif

    public const int DefaultTravelDistance = 20;
    public const float DefaultLatitude = 39.0919879f;
    public const float DefaultLongitude = -94.9053574f;
    public const string DefaultCity = "Kansas City";
    public const string DefaultRegion = "MO";
    public const string DefaultCountry = "USA";
}