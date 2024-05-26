namespace TrashMobMobile.Config;

using TrashMobMobile.Models;

public static class Settings
{
#if USETEST
    public const string ApiBaseUrl = "https://as-tm-dev-westus2.azurewebsites.net/api/";

    public const string SiteBaseUrl = "https://as-tm-dev-westus2.azurewebsites.net";
#else
    public const string ApiBaseUrl = "https://www.trashmob.eco/api/";

    public const string SiteBaseUrl = "https://www.trashmob.eco";
#endif

    public static WaiverVersion CurrentTrashMobWaiverVersion = new()
    {
        VersionId = "1.0",
        VersionDate = "2023-07-01 00:00:00"
    };

    public const int DefaultTravelDistance = 20;
    public const float DefaultLatitude = 39.0919879f;
    public const float DefaultLongitude = -94.9053574f;
    public const string DefaultCity = "Kansas City";
    public const string DefaultRegion = "MO";
    public const string DefaultCountry = "USA";
}