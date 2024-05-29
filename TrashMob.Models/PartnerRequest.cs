#nullable disable

namespace TrashMob.Models
{
    public class PartnerRequest : KeyedModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string Notes { get; set; }

        public int PartnerRequestStatusId { get; set; }

        public int PartnerTypeId { get; set; }

        public bool isBecomeAPartnerRequest { get; set; }

        public virtual PartnerRequestStatus PartnerRequestStatus { get; set; }

        public virtual PartnerType PartnerType { get; set; }
    }
}