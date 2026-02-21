#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a batch of email invitations sent by an admin, community, team, or user.
    /// </summary>
    public class EmailInviteBatch : KeyedModel
    {
        /// <summary>
        /// Gets or sets the sender's user identifier.
        /// </summary>
        public Guid SenderUserId { get; set; }

        /// <summary>
        /// Gets or sets the batch type (Admin, Community, Team, User).
        /// </summary>
        public string BatchType { get; set; }

        /// <summary>
        /// Gets or sets the community (partner) identifier (if community invite).
        /// </summary>
        public Guid? CommunityId { get; set; }

        /// <summary>
        /// Gets or sets the team identifier (if team invite).
        /// </summary>
        public Guid? TeamId { get; set; }

        #region Statistics

        /// <summary>
        /// Gets or sets the total number of invites in this batch.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the number of invites sent.
        /// </summary>
        public int SentCount { get; set; }

        /// <summary>
        /// Gets or sets the number of invites delivered.
        /// </summary>
        public int DeliveredCount { get; set; }

        /// <summary>
        /// Gets or sets the number of invites bounced.
        /// </summary>
        public int BouncedCount { get; set; }

        /// <summary>
        /// Gets or sets the number of invites that failed to send.
        /// </summary>
        public int FailedCount { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the batch status (Pending, Processing, Complete, Failed).
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets when processing completed.
        /// </summary>
        public DateTimeOffset? CompletedDate { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Gets or sets the sender user.
        /// </summary>
        public virtual User SenderUser { get; set; }

        /// <summary>
        /// Gets or sets the community (partner) if this is a community invite.
        /// </summary>
        public virtual Partner Community { get; set; }

        /// <summary>
        /// Gets or sets the team if this is a team invite.
        /// </summary>
        public virtual Team Team { get; set; }

        /// <summary>
        /// Gets or sets the collection of individual invites in this batch.
        /// </summary>
        public virtual ICollection<EmailInvite> Invites { get; set; }

        #endregion
    }
}
