namespace TrashMob.Shared.Extensions
{
    using TrashMob.Models;

    public static class PickupLocationExtensions
    {
        public static string GoogleMapsUrl(this PickupLocation pickupLocation)
        {
            return
                $"https://google.com/maps/place/{pickupLocation.StreetAddress}+{pickupLocation.City}+{pickupLocation.Region}+{pickupLocation.PostalCode}+{pickupLocation.Country}";
        }
    }
}