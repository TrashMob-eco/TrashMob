#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a team of volunteers who organize and participate in cleanup events together.
    /// </summary>
    /// <remarks>
    /// Teams provide social cohesion, long-term engagement, and friendly competition among volunteers.
    /// Teams can be public (discoverable via map and search) or private (invite-only, hidden from discovery).
    /// Team leads have management permissions including member approval and event creation.
    /// </remarks>
    public class Team : KeyedModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Team"/> class.
        /// </summary>
        public Team()
        {
            Members = [];
            JoinRequests = [];
            TeamEvents = [];
            Photos = [];
        }

        /// <summary>
        /// Gets or sets the name of the team. Must be globally unique (case-insensitive).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the team.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the URL of the team's logo image.
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the team is publicly visible.
        /// Public teams appear on the map and in search results.
        /// Private teams are invite-only and hidden from discovery.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether joining the team requires approval from a lead.
        /// </summary>
        public bool RequiresApproval { get; set; }

        /// <summary>
        /// Gets or sets the latitude coordinate of the team's primary location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate of the team's primary location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the city where the team is based.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the region or state where the team is based.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the country where the team is based.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the team's location.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the team is active.
        /// Inactive teams are soft-deleted and can be reactivated.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the collection of team members.
        /// </summary>
        public virtual ICollection<TeamMember> Members { get; set; }

        /// <summary>
        /// Gets or sets the collection of pending join requests.
        /// </summary>
        public virtual ICollection<TeamJoinRequest> JoinRequests { get; set; }

        /// <summary>
        /// Gets or sets the collection of events associated with this team.
        /// </summary>
        public virtual ICollection<TeamEvent> TeamEvents { get; set; }

        /// <summary>
        /// Gets or sets the collection of photos in the team's gallery.
        /// </summary>
        public virtual ICollection<TeamPhoto> Photos { get; set; }
    }
}
