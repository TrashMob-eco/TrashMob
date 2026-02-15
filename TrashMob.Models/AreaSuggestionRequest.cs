namespace TrashMob.Models
{
    public class AreaSuggestionRequest
    {
        public string Description { get; set; } = string.Empty;

        public double? CenterLatitude { get; set; }

        public double? CenterLongitude { get; set; }

        public string? CommunityName { get; set; }

        public double? BoundsNorth { get; set; }

        public double? BoundsSouth { get; set; }

        public double? BoundsEast { get; set; }

        public double? BoundsWest { get; set; }
    }
}
