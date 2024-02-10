namespace TrashMobMobile.Config
{
    using TrashMobMobile.Models;

    public class Settings
    {
        public string ApiBaseUrl { get; set; }

        public string SiteBaseUrl { get; set; }

        public WaiverVersion CurrentTrashMobWaiverVersion { get; set; }
    }
}