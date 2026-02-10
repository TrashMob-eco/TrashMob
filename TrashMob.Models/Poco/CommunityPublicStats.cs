namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Aggregate public statistics for all active communities.
    /// </summary>
    public class CommunityPublicStats
    {
        /// <summary>
        /// Gets or sets the total number of active communities.
        /// </summary>
        public int TotalCommunities { get; set; }

        /// <summary>
        /// Gets or sets the total number of events across all communities.
        /// </summary>
        public int TotalCommunityEvents { get; set; }

        /// <summary>
        /// Gets or sets the total number of volunteers across all community events.
        /// </summary>
        public int TotalCommunityVolunteers { get; set; }
    }
}
