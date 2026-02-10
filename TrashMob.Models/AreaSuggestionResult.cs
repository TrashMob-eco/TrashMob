namespace TrashMob.Models
{
    public class AreaSuggestionResult
    {
        public string? GeoJson { get; set; }

        public string? SuggestedName { get; set; }

        public string? SuggestedAreaType { get; set; }

        public double Confidence { get; set; }

        public string? Message { get; set; }
    }
}
