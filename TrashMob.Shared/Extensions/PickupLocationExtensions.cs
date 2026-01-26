namespace TrashMob.Shared.Extensions
{
    using TrashMob.Models;

    /// <summary>
    /// Extension methods for the PickupLocation class.
    /// </summary>
    public static class PickupLocationExtensions
    {
        /// <summary>
        /// Generates a Google Maps URL for the pickup location.
        /// </summary>
        /// <param name="pickupLocation">The pickup location to generate the URL for.</param>
        /// <returns>A Google Maps URL string for the pickup location's address.</returns>
        public static string GoogleMapsUrl(this PickupLocation pickupLocation)
        {
            return
                $"https://google.com/maps/place/{pickupLocation.StreetAddress}+{pickupLocation.City}+{pickupLocation.Region}+{pickupLocation.PostalCode}+{pickupLocation.Country}";
        }
    }
}