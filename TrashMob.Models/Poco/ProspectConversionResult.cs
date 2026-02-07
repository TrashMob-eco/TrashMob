#nullable enable

namespace TrashMob.Models.Poco
{
    using System;

    public class ProspectConversionResult
    {
        public bool Success { get; set; }

        public Guid PartnerId { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
