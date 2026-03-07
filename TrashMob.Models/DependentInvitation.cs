#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents an invitation for a minor dependent to create their own TrashMob account.
    /// </summary>
    /// <remarks>
    /// Parents create invitations for their 13+ dependents. The invitation includes a secure token
    /// sent via email. When the minor creates an account using that email, the system automatically
    /// links their User record to the parent via the auth handler.
    /// </remarks>
    public class DependentInvitation : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the dependent being invited.
        /// </summary>
        public Guid DependentId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the parent user who sent the invitation.
        /// </summary>
        public Guid ParentUserId { get; set; }

        /// <summary>
        /// Gets or sets the email address the invitation was sent to.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the SHA-256 hash of the invitation token (plain token is only in the email).
        /// </summary>
        public string TokenHash { get; set; }

        /// <summary>
        /// Gets or sets the invitation status (New, Sent, Accepted, Canceled).
        /// </summary>
        public int InvitationStatusId { get; set; }

        /// <summary>
        /// Gets or sets the date when the invitation was created.
        /// </summary>
        public DateTimeOffset DateInvited { get; set; }

        /// <summary>
        /// Gets or sets the date when the invitation expires.
        /// </summary>
        public DateTimeOffset ExpiresDate { get; set; }

        /// <summary>
        /// Gets or sets the date when the invitation was accepted.
        /// </summary>
        public DateTimeOffset? DateAccepted { get; set; }

        /// <summary>
        /// Gets or sets the user ID of the minor who accepted the invitation and created an account.
        /// </summary>
        public Guid? AcceptedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the dependent being invited.
        /// </summary>
        public virtual Dependent Dependent { get; set; }

        /// <summary>
        /// Gets or sets the parent user who sent the invitation.
        /// </summary>
        public virtual User ParentUser { get; set; }

        /// <summary>
        /// Gets or sets the minor user who accepted the invitation.
        /// </summary>
        public virtual User AcceptedByUser { get; set; }

        /// <summary>
        /// Gets or sets the invitation status.
        /// </summary>
        public virtual InvitationStatus InvitationStatus { get; set; }
    }
}
