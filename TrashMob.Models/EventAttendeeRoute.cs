#nullable disable

namespace TrashMob.Models
{
    using NetTopologySuite.Geometries;

    /// <summary>
    /// Represents the route taken by an attendee during a cleanup event.
    /// </summary>
    public class EventAttendeeRoute : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who took this route.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the geometric path representing the user's route during the event.
        /// </summary>
        public Geometry UserPath { get; set; }

        /// <summary>
        /// Gets or sets the start time of the route tracking.
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the route tracking.
        /// </summary>
        public DateTimeOffset EndTime { get; set; }

        /// <summary>
        /// Gets or sets the event associated with this route.
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// Gets or sets the user who took this route.
        /// </summary>
        public virtual User User { get; set; }
    }
}