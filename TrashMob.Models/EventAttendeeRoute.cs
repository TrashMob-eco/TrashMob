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
        /// Initializes a new instance of the <see cref="EventAttendeeRoute"/> class.
        /// </summary>
        public EventAttendeeRoute()
        {
            RoutePoints = new HashSet<RoutePoint>();
        }

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

        #region Distance & Duration

        /// <summary>
        /// Gets or sets the total distance in meters.
        /// </summary>
        public int TotalDistanceMeters { get; set; }

        /// <summary>
        /// Gets or sets the duration in minutes.
        /// </summary>
        public int DurationMinutes { get; set; }

        #endregion

        #region Privacy Settings

        /// <summary>
        /// Gets or sets the privacy level (Private, EventOnly, Public).
        /// </summary>
        public string PrivacyLevel { get; set; } = "EventOnly";

        /// <summary>
        /// Gets or sets whether the route has been trimmed for privacy.
        /// </summary>
        public bool IsTrimmed { get; set; }

        /// <summary>
        /// Gets or sets meters trimmed from the start.
        /// </summary>
        public int TrimStartMeters { get; set; } = 100;

        /// <summary>
        /// Gets or sets meters trimmed from the end.
        /// </summary>
        public int TrimEndMeters { get; set; } = 100;

        #endregion

        #region Route Metrics

        /// <summary>
        /// Gets or sets bags collected along this route.
        /// </summary>
        public int? BagsCollected { get; set; }

        /// <summary>
        /// Gets or sets weight collected along this route.
        /// </summary>
        public decimal? WeightCollected { get; set; }

        /// <summary>
        /// Gets or sets the weight unit for the weight collected value.
        /// </summary>
        public int? WeightUnitId { get; set; }

        /// <summary>
        /// Gets or sets notes about this route.
        /// </summary>
        public string Notes { get; set; }

        #endregion

        #region Decay

        /// <summary>
        /// Gets or sets when this route expires for public viewing.
        /// </summary>
        public DateTimeOffset? ExpiresDate { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the event associated with this route.
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// Gets or sets the user who took this route.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the collection of detailed GPS route points.
        /// </summary>
        public virtual ICollection<RoutePoint> RoutePoints { get; set; }
    }
}