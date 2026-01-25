namespace TrashMob.Shared.Extensions
{
    using TrashMob.Models;

    /// <summary>
    /// Extension methods for the LitterImage class.
    /// </summary>
    public static class LitterImageExtensions
    {
        /// <summary>
        /// Generates a Google Maps URL for the litter image location.
        /// </summary>
        /// <param name="litterImage">The litter image to generate the URL for.</param>
        /// <returns>A Google Maps URL string for the litter image's address.</returns>
        public static string GoogleMapsUrl(this LitterImage litterImage)
        {
            return
                $"https://google.com/maps/place/{litterImage.StreetAddress}+{litterImage.City}+{litterImage.Region}+{litterImage.PostalCode}+{litterImage.Country}";
        }

        /// <summary>
        /// Formats the litter image's address for display.
        /// </summary>
        /// <param name="litterImage">The litter image to format the address for.</param>
        /// <returns>A formatted address string.</returns>
        public static string DisplayAddress(this LitterImage litterImage)
        {
            return
                $"{litterImage.StreetAddress}, {litterImage.City}, {litterImage.Region}, {litterImage.PostalCode}, {litterImage.Country}";
        }
    }
}