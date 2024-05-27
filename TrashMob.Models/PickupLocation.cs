#nullable disable

namespace TrashMob.Models
{
    public class PickupLocation : KeyedModel
    {
        public Guid EventId { get; set; }

        public string StreetAddress { get; set; } = "";

        public string City { get; set; } = "";

        public string Region { get; set; } = "";

        public string PostalCode { get; set; } = "";

        public string Country { get; set; } = "";

        public string County { get; set; } = "";

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool HasBeenSubmitted { get; set; } = false;

        public bool HasBeenPickedUp { get; set; } = false;

        public string Notes { get; set; } = "";

        public string Name { get; set; } = "";

        public virtual Event Event { get; set; }
    }
}