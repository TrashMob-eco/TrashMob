namespace TrashMob.Models
{
    public class AreaSuggestionRequest
    {
        public string Description { get; set; } = string.Empty;

        public double? CenterLatitude { get; set; }

        public double? CenterLongitude { get; set; }

        public string? CommunityName { get; set; }
    }
}
