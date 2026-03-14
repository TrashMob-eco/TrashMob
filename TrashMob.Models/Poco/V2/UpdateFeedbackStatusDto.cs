namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// DTO for admin updating feedback status.
    /// </summary>
    public class UpdateFeedbackStatusDto
    {
        /// <summary>Gets or sets the new status (New, Reviewed, Resolved, Deferred).</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Gets or sets internal admin notes.</summary>
        public string? InternalNotes { get; set; }
    }
}
