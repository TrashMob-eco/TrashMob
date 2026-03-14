namespace TrashMob.Models.Poco.V2
{
    public class PartnerContactDto
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
