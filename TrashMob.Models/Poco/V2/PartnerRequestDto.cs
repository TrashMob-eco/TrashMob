namespace TrashMob.Models.Poco.V2
{
    public class PartnerRequestDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Notes { get; set; }
        public int PartnerRequestStatusId { get; set; }
        public int PartnerTypeId { get; set; }
        public bool IsBecomeAPartnerRequest { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
