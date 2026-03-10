#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a dependent invitation. Excludes security-sensitive fields (token hash).
    /// </summary>
    public class DependentInvitationDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the invitation.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the dependent identifier.
        /// </summary>
        public Guid DependentId { get; set; }

        /// <summary>
        /// Gets or sets the parent user identifier.
        /// </summary>
        public Guid ParentUserId { get; set; }

        /// <summary>
        /// Gets or sets the email address the invitation was sent to.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the invitation status identifier.
        /// </summary>
        public int InvitationStatusId { get; set; }

        /// <summary>
        /// Gets or sets the date the invitation was sent.
        /// </summary>
        public DateTimeOffset DateInvited { get; set; }

        /// <summary>
        /// Gets or sets the date the invitation expires.
        /// </summary>
        public DateTimeOffset ExpiresDate { get; set; }

        /// <summary>
        /// Gets or sets the date the invitation was accepted, if applicable.
        /// </summary>
        public DateTimeOffset? DateAccepted { get; set; }

        /// <summary>
        /// Gets or sets the user identifier of who accepted the invitation, if applicable.
        /// </summary>
        public Guid? AcceptedByUserId { get; set; }
    }
}
