namespace TrashMobMobile.Extensions;

using TrashMob.Models;

public static class UserExtensions
{
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
            Location = new Location(user.Latitude ?? Config.Settings.DefaultLatitude, user.Longitude ?? Config.Settings.DefaultLongitude),
        };
    }
}
