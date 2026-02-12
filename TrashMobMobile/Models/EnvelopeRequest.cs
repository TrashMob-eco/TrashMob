namespace TrashMobMobile.Models
{
    public class EnvelopeRequest
    {
        public string SignerEmail { get; set; } = string.Empty;

        public string SignerName { get; set; } = string.Empty;

        public Guid CreatedByUserId { get; set; }

        public string ReturnUrl { get; set; } = string.Empty;

        public string PingUrl { get; set; } = string.Empty;
    }
}