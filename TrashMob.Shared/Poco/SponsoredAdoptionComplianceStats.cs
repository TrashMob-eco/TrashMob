namespace TrashMob.Shared.Poco
{
    /// <summary>
    /// Represents compliance statistics for a community's sponsored adoption program.
    /// </summary>
    public class SponsoredAdoptionComplianceStats
    {
        /// <summary>
        /// Gets or sets the total number of sponsored adoptions.
        /// </summary>
        public int TotalSponsoredAdoptions { get; set; }

        /// <summary>
        /// Gets or sets the number of active sponsored adoptions.
        /// </summary>
        public int ActiveAdoptions { get; set; }

        /// <summary>
        /// Gets or sets the number of expired sponsored adoptions.
        /// </summary>
        public int ExpiredAdoptions { get; set; }

        /// <summary>
        /// Gets or sets the number of terminated sponsored adoptions.
        /// </summary>
        public int TerminatedAdoptions { get; set; }

        /// <summary>
        /// Gets or sets the number of active adoptions that are on schedule (last cleanup within frequency window).
        /// </summary>
        public int AdoptionsOnSchedule { get; set; }

        /// <summary>
        /// Gets or sets the number of active adoptions that are overdue for cleanup.
        /// </summary>
        public int AdoptionsOverdue { get; set; }

        /// <summary>
        /// Gets or sets the total number of professional cleanup logs across all adoptions.
        /// </summary>
        public int TotalCleanupLogs { get; set; }

        /// <summary>
        /// Gets or sets the total weight collected in pounds across all cleanup logs.
        /// </summary>
        public decimal TotalWeightCollectedPounds { get; set; }

        /// <summary>
        /// Gets or sets the total bags collected across all cleanup logs.
        /// </summary>
        public int TotalBagsCollected { get; set; }

        /// <summary>
        /// Gets the compliance rate as a percentage of active adoptions that are on schedule.
        /// </summary>
        public double ComplianceRate => ActiveAdoptions > 0
            ? (double)AdoptionsOnSchedule / ActiveAdoptions * 100
            : 0;
    }
}
