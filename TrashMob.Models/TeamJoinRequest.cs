#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a request from a user to join a team.
    /// </summary>
    /// <remarks>
    /// Join requests are created when a user requests to join a public team that requires approval,
    /// or when they attempt to join a private team (which is blocked). Team leads can approve
    /// or reject pending requests.
    /// </remarks>
    public class TeamJoinRequest : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the team being requested to join.
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user requesting to join.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the date when the join request was submitted.
        /// </summary>
        public DateTimeOffset RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the status of the join request.
        /// Valid values: "Pending", "Approved", "Rejected".
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who reviewed the request.
        /// </summary>
        public Guid? ReviewedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date when the request was reviewed.
        /// </summary>
        public DateTimeOffset? ReviewedDate { get; set; }

        /// <summary>
        /// Gets or sets the team this request is for.
        /// </summary>
        public virtual Team Team { get; set; }

        /// <summary>
        /// Gets or sets the user who submitted the request.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the user who reviewed the request.
        /// </summary>
        public virtual User ReviewedByUser { get; set; }
    }
}
