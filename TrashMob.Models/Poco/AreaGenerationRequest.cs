#nullable enable

namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Request to start an area generation batch.
    /// </summary>
    public class AreaGenerationRequest
    {
        /// <summary>
        /// Gets or sets the category to search for (e.g. "School", "Park").
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets optional sub-region bounds for the search.
        /// If null, uses the community's full geographic bounds.
        /// </summary>
        public double? BoundsNorth { get; set; }

        public double? BoundsSouth { get; set; }

        public double? BoundsEast { get; set; }

        public double? BoundsWest { get; set; }
    }

    /// <summary>
    /// Request to bulk approve or reject staged areas.
    /// </summary>
    public class BulkReviewRequest
    {
        /// <summary>
        /// Gets or sets the batch ID.
        /// </summary>
        public System.Guid BatchId { get; set; }

        /// <summary>
        /// Gets or sets optional specific area IDs. If null, applies to all pending areas in the batch.
        /// </summary>
        public System.Collections.Generic.List<System.Guid>? Ids { get; set; }
    }

    /// <summary>
    /// Request to update a staged area's name.
    /// </summary>
    public class UpdateStagedAreaNameRequest
    {
        /// <summary>
        /// Gets or sets the new name.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
