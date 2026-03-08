#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// V2 API representation of platform-wide or per-user aggregate statistics.
    /// </summary>
    public class StatsDto
    {
        /// <summary>
        /// Gets or sets the total number of bags of litter collected.
        /// </summary>
        public int TotalBags { get; set; }

        /// <summary>
        /// Gets or sets the total number of volunteer hours contributed.
        /// </summary>
        public int TotalHours { get; set; }

        /// <summary>
        /// Gets or sets the total number of cleanup events held.
        /// </summary>
        public int TotalEvents { get; set; }

        /// <summary>
        /// Gets or sets the total weight of litter collected in pounds.
        /// </summary>
        public decimal TotalWeightInPounds { get; set; }

        /// <summary>
        /// Gets or sets the total weight of litter collected in kilograms.
        /// </summary>
        public decimal TotalWeightInKilograms { get; set; }

        /// <summary>
        /// Gets or sets the total number of participants across all events.
        /// </summary>
        public int TotalParticipants { get; set; }

        /// <summary>
        /// Gets or sets the total number of litter reports submitted.
        /// </summary>
        public int TotalLitterReportsSubmitted { get; set; }

        /// <summary>
        /// Gets or sets the total number of litter reports that have been closed.
        /// </summary>
        public int TotalLitterReportsClosed { get; set; }
    }
}
