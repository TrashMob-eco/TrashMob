namespace TrashMobMobileApp.Extensions
{
    using TrashMob.Models;
    using TrashMobMobileApp.Models;

    public static class UserExtensions
    {
        public static bool HasUserSignedWaiver(this User user, Waiver waiver, WaiverVersion waiverVersion)
        {
            // If there is no waiver, skip it
            if (waiver == null)
            {
                return true;
            }

            if (waiver.IsWaiverEnabled && user.DateAgreedToTrashMobWaiver < DateTime.Parse(waiverVersion.VersionDate))
            {
                return false;
            }

            return true;
        }
    }
}