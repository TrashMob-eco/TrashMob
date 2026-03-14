namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// DTO for user's email invite quota information.
    /// </summary>
    public class EmailInviteQuotaDto
    {
        /// <summary>Gets or sets the maximum emails per batch.</summary>
        public int MaxPerBatch { get; set; }

        /// <summary>Gets or sets the maximum invites per month.</summary>
        public int MaxPerMonth { get; set; }

        /// <summary>Gets or sets the number used this month.</summary>
        public int UsedThisMonth { get; set; }

        /// <summary>Gets or sets the remaining invites this month.</summary>
        public int RemainingThisMonth { get; set; }
    }
}
