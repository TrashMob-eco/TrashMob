namespace TrashMobMobile.Config;

using TrashMobMobile.Models;

public static class Settings
{
#if DEBUG
    public const string ApiBaseUrl = "https://as-tm-dev-westus2.azurewebsites.net/api/";

    public const string SiteBaseUrl = "https://as-tm-dev-westus2.azurewebsites.net";
#else
    public const string ApiBaseUrl = "https://www.trashmob.eco/api/";

    public const string SiteBaseUrl = "https://www.trashmob.eco";
#endif

    public static WaiverVersion CurrentTrashMobWaiverVersion = new WaiverVersion
    {
        VersionId = "1.0",
        VersionDate = "2023-07-01 00:00:00"
    };
}