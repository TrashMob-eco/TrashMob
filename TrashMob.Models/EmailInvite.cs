#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents an individual email invitation with tracking.
    /// </summary>
    public class EmailInvite : KeyedModel
    {
        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        public Guid BatchId { get; set; }

        /// <summary>
        /// Gets or sets the recipient email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the invite status (Pending, Sent, Delivered, Bounced, Failed).
        /// </summary>
        public string Status { get; set; } = "Pending";

        #region Tracking Dates

        /// <summary>
        /// Gets or sets when the invite was sent.
        /// </summary>
        public DateTimeOffset? SentDate { get; set; }

        /// <summary>
        /// Gets or sets when the invite was delivered.
        /// </summary>
        public DateTimeOffset? DeliveredDate { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets an error message if the send failed.
        /// </summary>
        public string ErrorMessage { get; set; }

        #region Conversion Tracking

        /// <summary>
        /// Gets or sets the user ID if the invite converted to a signup.
        /// </summary>
        public Guid? SignedUpUserId { get; set; }

        /// <summary>
        /// Gets or sets when the invited person signed up.
        /// </summary>
        public DateTimeOffset? SignedUpDate { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets or sets the parent batch.
        /// </summary>
        public virtual EmailInviteBatch Batch { get; set; }

        /// <summary>
        /// Gets or sets the user who signed up from this invite.
        /// </summary>
        public virtual User SignedUpUser { get; set; }

        #endregion
    }
}
