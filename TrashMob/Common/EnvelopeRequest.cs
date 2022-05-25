namespace TrashMob.Common
{
    using System;

    public class EnvelopeRequest
    {
        public string AccountId { get; set; }

        public string SignerEmail{ get; set; }

        public string SignerName { get; set; }

        public Guid SignerClientId { get; set; }

        public string AccessToken { get; set; }

        public string TemplateId { get; set; }

        public string BasePath { get; set; }

        public string ReturnUrl { get; set; }

        public string PingUrl { get; set; }
    }
}
