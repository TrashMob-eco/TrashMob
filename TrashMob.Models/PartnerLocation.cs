#nullable disable

namespace TrashMob.Models
{
    public class PartnerLocation : KeyedModel
    {
        public PartnerLocation()
        {
            PartnerLocationContacts = new HashSet<PartnerLocationContact>();
            PartnerLocationServices = new HashSet<PartnerLocationService>();
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

        public string PublicNotes { get; set; }

        public string PrivateNotes { get; set; }

        public bool IsActive { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual ICollection<PartnerLocationContact> PartnerLocationContacts { get; set; }

        public virtual ICollection<PartnerLocationService> PartnerLocationServices { get; set; }
    }
}