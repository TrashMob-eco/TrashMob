namespace TrashMobMobileApp.Config
{
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Models;

    public class Settings
    {
        public string ApiBaseUrl { get; set; }

        public string SiteBaseUrl { get; set; }

        public B2CConstants B2CConstants { get; set; }

        public WaiverVersion CurrentTrashMobWaiverVersion { get; set; }
    }
}