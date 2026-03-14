namespace TrashMob.Models.Poco.V2
{
    public class PartnerAdminDto
    {
        public Guid PartnerId { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
