namespace TrashMob.Models.Poco.V2
{
    public class PartnerSocialMediaAccountDto
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public string AccountIdentifier { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public int SocialMediaAccountTypeId { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
