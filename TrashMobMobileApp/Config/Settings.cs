namespace TrashMobMobileApp.Config
{
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Models;

    public class Settings
    {
        public string ApiBaseUrl { get; set; }

        public string SiteBaseUrl { get; set; }

        public AzureADB2CConfig AzureADB2C { get; set; }

        public DownStreamApiConfig DownStreamApi { get; set; }

        public WaiverVersion CurrentTrashMobWaiverVersion { get; set; }
    }
}