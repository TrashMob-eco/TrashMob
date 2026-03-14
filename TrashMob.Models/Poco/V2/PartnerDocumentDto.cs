namespace TrashMob.Models.Poco.V2
{
    public class PartnerDocumentDto
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? ContentType { get; set; }
        public long? FileSizeBytes { get; set; }
        public int DocumentTypeId { get; set; }
        public DateTimeOffset? ExpirationDate { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
