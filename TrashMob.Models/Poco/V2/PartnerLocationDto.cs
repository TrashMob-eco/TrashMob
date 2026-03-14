namespace TrashMob.Models.Poco.V2
{
    public class PartnerLocationDto
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? PublicNotes { get; set; }
        public string? PrivateNotes { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
