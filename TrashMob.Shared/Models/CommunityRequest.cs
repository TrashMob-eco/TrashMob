#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class CommunityRequest
    {
        public CommunityRequest()
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string ContactName { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public virtual User CreatedByUser { get; set; }
    }
}
