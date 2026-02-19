namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Result of a bulk area clear operation.
    /// </summary>
    public class AreaBulkClearResult
    {
        /// <summary>
        /// Gets or sets the number of adoptable areas deactivated.
        /// </summary>
        public int AreasDeactivated { get; set; }

        /// <summary>
        /// Gets or sets the number of staged areas deleted.
        /// </summary>
        public int StagedAreasDeleted { get; set; }

        /// <summary>
        /// Gets or sets the number of generation batches deleted.
        /// </summary>
        public int BatchesDeleted { get; set; }
    }
}
