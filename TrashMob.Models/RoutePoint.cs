#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a single GPS point along an attendee's route.
    /// </summary>
    public class RoutePoint
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the parent route identifier.
        /// </summary>
        public Guid RouteId { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the altitude in meters.
        /// </summary>
        public double? Altitude { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of this point.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the GPS accuracy in meters.
        /// </summary>
        public double? Accuracy { get; set; }

        /// <summary>
        /// Gets or sets the parent route.
        /// </summary>
        public virtual EventAttendeeRoute Route { get; set; }
    }
}
