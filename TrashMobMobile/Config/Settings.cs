namespace TrashMobMobile.Config;

using TrashMobMobile.Models;

public static class Settings
{
    // TODO: replace with PROD values in release pipeline
    public const string ApiBaseUrl = "https://as-tm-dev-westus2.azurewebsites.net/api/";

    public const string SiteBaseUrl = "https://as-tm-dev-westus2.azurewebsites.net";

    public static WaiverVersion CurrentTrashMobWaiverVersion = new WaiverVersion
    {
        VersionId = "1.0",
        VersionDate = "2023-07-01 00:00:00"
    };
}