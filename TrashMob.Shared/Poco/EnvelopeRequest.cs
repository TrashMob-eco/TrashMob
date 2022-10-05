namespace TrashMob.Shared.Poco
{
    using TrashMob.Models;

    public class EnvelopeRequest : BaseModel
    {
        public string SignerEmail { get; set; }

        public string SignerName { get; set; }

        public string ReturnUrl { get; set; }

        public string PingUrl { get; set; }
    }
}
