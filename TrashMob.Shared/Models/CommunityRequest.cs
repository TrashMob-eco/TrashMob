#nullable disable

namespace TrashMob.Shared.Models
{
    public partial class CommunityRequest : KeyedModel
    {
        public CommunityRequest()
        {
        }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string ContactName { get; set; }

        public string Website { get; set; }
    }
}
