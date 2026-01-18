namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents filter criteria for querying events.
    /// </summary>
    public class EventFilter : GeneralFilter
    {
        /// <summary>
        /// Gets or sets the optional event status identifier to filter by.
        /// </summary>
        public int? EventStatusId { get; set; }
    }
}