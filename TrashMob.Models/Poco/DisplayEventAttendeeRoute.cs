namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents an event attendee route for display purposes with location points instead of geometric path.
    /// </summary>
    public class DisplayEventAttendeeRoute
    {
        /// <summary>
        /// Gets or sets the unique identifier for this route.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who took this route.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the start time of the route tracking.
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the route tracking.
        /// </summary>
        public DateTimeOffset EndTime { get; set; }

        /// <summary>
        /// Gets or sets the list of location points that make up the route, sorted by order.
        /// </summary>
        public List<SortableLocation> Locations { get; set; } = [];
    }
}