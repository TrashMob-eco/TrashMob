namespace TrashMobMobileApp.Models
{
    public class EnvelopeRequest
    {
        public string SignerEmail { get; set; }

        public string SignerName { get; set; }

        public Guid CreatedByUserId { get; set; }

        public string ReturnUrl { get; set; }

        public string PingUrl { get; set; }
    }
}
