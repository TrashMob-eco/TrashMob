namespace TrashMob.Models.Poco.V2
{
    public class PartnerAdminInvitationDto
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public string Email { get; set; } = string.Empty;
        public int InvitationStatusId { get; set; }
        public DateTimeOffset DateInvited { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
