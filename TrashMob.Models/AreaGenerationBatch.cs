#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a batch job that discovers and stages adoptable areas from OSM data.
    /// </summary>
    public class AreaGenerationBatch : KeyedModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AreaGenerationBatch"/> class.
        /// </summary>
        public AreaGenerationBatch()
        {
            StagedAreas = [];
        }

        /// <summary>
        /// Gets or sets the community (partner) this batch belongs to.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the category of areas to discover (e.g., "School", "Park").
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the batch status (Queued, Discovering, Processing, Complete, Failed, Cancelled).
        /// </summary>
        public string Status { get; set; } = "Queued";

        #region Progress Counters

        /// <summary>
        /// Gets or sets the number of features discovered from OSM.
        /// </summary>
        public int DiscoveredCount { get; set; }

        /// <summary>
        /// Gets or sets the number of features processed so far.
        /// </summary>
        public int ProcessedCount { get; set; }

        /// <summary>
        /// Gets or sets the number of features skipped (too small, too large, etc.).
        /// </summary>
        public int SkippedCount { get; set; }

        /// <summary>
        /// Gets or sets the number of features staged for review.
        /// </summary>
        public int StagedCount { get; set; }

        /// <summary>
        /// Gets or sets the number of staged areas approved by a reviewer.
        /// </summary>
        public int ApprovedCount { get; set; }

        /// <summary>
        /// Gets or sets the number of staged areas rejected by a reviewer.
        /// </summary>
        public int RejectedCount { get; set; }

        /// <summary>
        /// Gets or sets the number of approved areas promoted to real adoptable areas.
        /// </summary>
        public int CreatedCount { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the error message if the batch failed.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets when the batch completed (or failed/cancelled).
        /// </summary>
        public DateTimeOffset? CompletedDate { get; set; }

        #region Optional Sub-Region Bounds

        /// <summary>
        /// Gets or sets the northern latitude bound for sub-region filtering.
        /// </summary>
        public double? BoundsNorth { get; set; }

        /// <summary>
        /// Gets or sets the southern latitude bound for sub-region filtering.
        /// </summary>
        public double? BoundsSouth { get; set; }

        /// <summary>
        /// Gets or sets the eastern longitude bound for sub-region filtering.
        /// </summary>
        public double? BoundsEast { get; set; }

        /// <summary>
        /// Gets or sets the western longitude bound for sub-region filtering.
        /// </summary>
        public double? BoundsWest { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets or sets the community (partner) that owns this batch.
        /// </summary>
        public virtual Partner Partner { get; set; }

        /// <summary>
        /// Gets or sets the collection of staged areas produced by this batch.
        /// </summary>
        public virtual ICollection<StagedAdoptableArea> StagedAreas { get; set; }

        #endregion
    }
}
