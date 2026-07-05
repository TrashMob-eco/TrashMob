#nullable disable

namespace TrashMob.Models
{
    using System;

    /// <summary>
    /// Distribution-list entry for the weekly and monthly municipal sales
    /// pipeline emails (Project 63 Phase 4b). One row per user; per-cadence
    /// opt-in flags let a subscriber take only weekly or only monthly.
    /// </summary>
    public class SalesReportSubscriber : KeyedModel
    {
        /// <summary>
        /// Gets or sets the subscribed <see cref="User"/> id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subscriber receives
        /// the weekly email. Defaults to true.
        /// </summary>
        public bool IncludeWeekly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subscriber receives
        /// the monthly email. Defaults to true.
        /// </summary>
        public bool IncludeMonthly { get; set; }

        /// <summary>
        /// Gets or sets the subscribed user (navigation).
        /// </summary>
        public virtual User User { get; set; }
    }
}
