
namespace TrashMob.Models.Poco
{
    using Microsoft.AspNetCore.Http;
    using System;

    public class FullLitterImage
    {
        public Guid Id { get; set; }

        public Guid LitterReportId { get; set; }

        public string ImageURL { get; set; } = string.Empty;

        public string StreetAddress { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }
    }
}