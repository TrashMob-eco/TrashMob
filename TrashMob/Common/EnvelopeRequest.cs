namespace TrashMob.Common
{
    using System;

    public class EnvelopeRequest
    {
        public string SignerEmail{ get; set; }

        public string SignerName { get; set; }

        public Guid SignerClientId { get; set; }

        public string BasePath { get; set; }

        public string ReturnUrl { get; set; }

        public string PingUrl { get; set; }
    }
}
