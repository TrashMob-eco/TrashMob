#nullable enable

namespace TrashMob.Models.Poco
{
    using System;

    public class ProspectConversionRequest
    {
        public Guid ProspectId { get; set; }

        public int PartnerTypeId { get; set; } = 1;

        public bool SendWelcomeEmail { get; set; } = true;
    }
}
