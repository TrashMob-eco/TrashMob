namespace TrashMobMobile.Extensions
{
    using TrashMob.Models;
    using TrashMobMobile.Models;

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

        public static AddressViewModel GetAddress(this User user)
        {
            return new AddressViewModel
            {
                City = user.City,
                Country = user.Country,
                Latitude = user.Latitude,
                Longitude = user.Longitude,
                PostalCode = user.PostalCode,
                Region = user.Region,
                Location = new Location(user.Latitude??0, user.Longitude??0)
            };
        }
    }
}