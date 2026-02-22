namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Pre-fill data for event summaries derived from route aggregates.
    /// </summary>
    public class EventSummaryPrefill
    {
        /// <summary>
        /// Gets or sets the total number of bags collected across all routes.
        /// </summary>
        public int NumberOfBags { get; set; }

        /// <summary>
        /// Gets or sets the total weight picked up, converted to the target unit.
        /// </summary>
        public decimal PickedWeight { get; set; }

        /// <summary>
        /// Gets or sets the weight unit used for the picked weight value.
        /// </summary>
        public int PickedWeightUnitId { get; set; }

        /// <summary>
        /// Gets or sets the total duration in minutes across all routes.
        /// </summary>
        public int DurationInMinutes { get; set; }

        /// <summary>
        /// Gets or sets the number of unique route contributors (attendee estimate).
        /// </summary>
        public int ActualNumberOfAttendees { get; set; }

        /// <summary>
        /// Gets or sets whether any route data exists for this event.
        /// </summary>
        public bool HasRouteData { get; set; }
    }
}
