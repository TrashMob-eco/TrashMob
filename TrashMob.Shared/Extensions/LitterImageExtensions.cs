namespace TrashMob.Shared.Extensions
{
    using TrashMob.Models;

    public static class LitterImageExtensions
    {      
        public static string GoogleMapsUrl(this LitterImage litterImage)
        {
            return $"https://google.com/maps/place/{litterImage.StreetAddress}+{litterImage.City}+{litterImage.Region}+{litterImage.PostalCode}+{litterImage.Country}";
        }

        public static string DisplayAddress(this LitterImage litterImage)
        {
            return $"{litterImage.StreetAddress}, {litterImage.City}, {litterImage.Region}, {litterImage.PostalCode}, {litterImage.Country}";
        }
    }
}
