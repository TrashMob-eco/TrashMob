namespace TrashMobMobile.Models
{
    using System;

    public class MobEvent
    {
        public Guid Id { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid LastUpdatedByUserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public int EventTypeId { get; set; }
        public int EventStatusId { get; set; }
        public int DurationHours { get; set; }
        public int DurationMinutes { get; set; }
        public int MaxNumberOfParticipants { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public bool IsEventPublic { get; set; }

        public string DisplayLocation
        {
            get
            {
                return $"{City}, {Region}, {Country}";
            }
        }
    }
}
