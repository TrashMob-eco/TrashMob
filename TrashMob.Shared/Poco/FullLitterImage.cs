
namespace TrashMob.Shared.Poco
{
    using Microsoft.AspNetCore.Http;
    using System;

    public class FullLitterImage
    {
        public IFormFile formFile {  get; set; }

        public Guid Id { get; set; }

        public Guid LitterReportId { get; set; }

        public string ImageURL { get; set; } = string.Empty;

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }
    }
}