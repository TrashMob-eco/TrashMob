#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a user's membership in a team.
    /// </summary>
    /// <remarks>
    /// A user can be a member of multiple teams. Team leads have elevated permissions
    /// to manage team settings, approve join requests, and invite new members.
    /// Minors (under 18) cannot be team leads due to organizational liability.
    /// </remarks>
    public class TeamMember : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the team.
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who is a member.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this member is a team lead.
        /// Team leads have management permissions for the team.
        /// </summary>
        public bool IsTeamLead { get; set; }

        /// <summary>
        /// Gets or sets the date when the user joined the team.
        /// </summary>
        public DateTimeOffset JoinedDate { get; set; }

        /// <summary>
        /// Gets or sets the team this membership belongs to.
        /// </summary>
        public virtual Team Team { get; set; }

        /// <summary>
        /// Gets or sets the user who is a member of the team.
        /// </summary>
        public virtual User User { get; set; }
    }
}
