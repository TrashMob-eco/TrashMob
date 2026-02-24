namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Request model for trimming a route's end time.
    /// </summary>
    public class TrimRouteTimeRequest
    {
        /// <summary>
        /// Gets or sets the new end time for the route. Must be between the route's start time and current end time.
        /// </summary>
        public DateTimeOffset NewEndTime { get; set; }
    }
}
