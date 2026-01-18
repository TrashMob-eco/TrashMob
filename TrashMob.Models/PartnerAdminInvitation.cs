#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents an invitation for a user to become an administrator of a partner organization.
    /// </summary>
    public class PartnerAdminInvitation : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the partner organization.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the email address of the invited user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the invitation status.
        /// </summary>
        public int InvitationStatusId { get; set; }

        /// <summary>
        /// Gets or sets the date when the invitation was sent.
        /// </summary>
        public DateTimeOffset DateInvited { get; set; }

        /// <summary>
        /// Gets or sets the partner organization.
        /// </summary>
        public virtual Partner Partner { get; set; }

        /// <summary>
        /// Gets or sets the status of the invitation.
        /// </summary>
        public virtual InvitationStatus InvitationStatus { get; set; }
    }
}