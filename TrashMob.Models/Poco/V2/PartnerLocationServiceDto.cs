namespace TrashMob.Models.Poco.V2
{
    public class PartnerLocationServiceDto
    {
        public Guid PartnerLocationId { get; set; }
        public int ServiceTypeId { get; set; }
        public bool IsAutoApproved { get; set; }
        public bool IsAdvanceNoticeRequired { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
