namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// Read DTO for email invite batches.
    /// </summary>
    public class EmailInviteBatchDto
    {
        /// <summary>Gets or sets the batch ID.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the sender's user ID.</summary>
        public Guid SenderUserId { get; set; }

        /// <summary>Gets or sets the batch type (Admin, Community, Team, User).</summary>
        public string BatchType { get; set; } = string.Empty;

        /// <summary>Gets or sets the total invite count.</summary>
        public int TotalCount { get; set; }

        /// <summary>Gets or sets the sent count.</summary>
        public int SentCount { get; set; }

        /// <summary>Gets or sets the delivered count.</summary>
        public int DeliveredCount { get; set; }

        /// <summary>Gets or sets the bounced count.</summary>
        public int BouncedCount { get; set; }

        /// <summary>Gets or sets the failed count.</summary>
        public int FailedCount { get; set; }

        /// <summary>Gets or sets the batch status.</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Gets or sets the completion date.</summary>
        public DateTimeOffset? CompletedDate { get; set; }

        /// <summary>Gets or sets the creation date.</summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
