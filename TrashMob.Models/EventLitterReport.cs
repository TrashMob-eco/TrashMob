#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents the association between an event and a litter report.
    /// </summary>
    public class EventLitterReport : BaseModel
    {
        /// <summary>
        /// Gets or sets the identifier of the associated event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the associated litter report.
        /// </summary>
        public Guid LitterReportId { get; set; }

        /// <summary>
        /// Gets or sets any notes about the litter report assignment to the event.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the associated event.
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// Gets or sets the associated litter report.
        /// </summary>
        public virtual LitterReport LitterReport { get; set; }
    }
}