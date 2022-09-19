#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class PartnerLocation : KeyedModel
    {
        public PartnerLocation()
        {
        }

        public Guid PartnerId { get; set; }

        public string Name { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string PrimaryEmail { get; set; }

        public string SecondaryEmail { get; set; }

        public string PrimaryPhone { get; set; }

        public string SecondaryPhone { get; set; }

        public string Notes { get; set; }

        public bool IsActive { get; set; }

        public virtual Partner Partner { get; set; }
    }
}
