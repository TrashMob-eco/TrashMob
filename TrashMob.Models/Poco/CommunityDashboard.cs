#nullable disable

namespace TrashMob.Models.Poco
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents dashboard data for community administrators.
    /// </summary>
    public class CommunityDashboard
    {
        /// <summary>
        /// Gets or sets the community/partner information.
        /// </summary>
        public Partner Community { get; set; }

        /// <summary>
        /// Gets or sets the aggregate stats for the community.
        /// </summary>
        public Stats Stats { get; set; }

        /// <summary>
        /// Gets or sets the list of recent events in the community.
        /// </summary>
        public IEnumerable<Event> RecentEvents { get; set; }

        /// <summary>
        /// Gets or sets the list of upcoming events in the community.
        /// </summary>
        public IEnumerable<Event> UpcomingEvents { get; set; }

        /// <summary>
        /// Gets or sets the count of teams near the community.
        /// </summary>
        public int TeamCount { get; set; }

        /// <summary>
        /// Gets or sets the count of open litter reports in the community.
        /// </summary>
        public int OpenLitterReportsCount { get; set; }

        /// <summary>
        /// Gets or sets the list of recent activity (events completed, teams joined, etc.).
        /// </summary>
        public IEnumerable<CommunityActivity> RecentActivity { get; set; }
    }

    /// <summary>
    /// Represents a recent activity item for the community dashboard.
    /// </summary>
    public class CommunityActivity
    {
        /// <summary>
        /// Gets or sets the type of activity (e.g., "EventCompleted", "TeamCreated", "LitterReportClosed").
        /// </summary>
        public string ActivityType { get; set; }

        /// <summary>
        /// Gets or sets a description of the activity.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets when the activity occurred.
        /// </summary>
        public DateTimeOffset ActivityDate { get; set; }

        /// <summary>
        /// Gets or sets an optional related entity ID (event ID, team ID, etc.).
        /// </summary>
        public Guid? RelatedEntityId { get; set; }
    }
}
