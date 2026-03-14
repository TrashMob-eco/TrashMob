#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a team member. Flat DTO with no navigation properties.
    /// </summary>
    public class TeamMemberDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the team membership.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the team identifier.
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the member's display username.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the member's given name.
        /// </summary>
        public string GivenName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the member's profile photo URL.
        /// </summary>
        public string ProfilePhotoUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the member is a team lead.
        /// </summary>
        public bool IsTeamLead { get; set; }

        /// <summary>
        /// Gets or sets the date the member joined the team.
        /// </summary>
        public DateTimeOffset JoinedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created this membership.
        /// </summary>
        public Guid CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when this membership was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last updated this membership.
        /// </summary>
        public Guid LastUpdatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when this membership was last updated.
        /// </summary>
        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}
