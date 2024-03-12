#nullable disable

namespace TrashMob.Models
{
    using System;

    public class LitterImage : KeyedModel
    {
        public Guid LitterReportId { get; set; }

        public string AzureBlobURL { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }
        
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool IsCancelled { get; set; }

        public virtual LitterReport LitterReport { get; set; }
    }
}