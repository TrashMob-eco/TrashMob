#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents an association between a team and an event.
    /// </summary>
    /// <remarks>
    /// This entity links teams to events they have created or are participating in.
    /// An event can be associated with multiple teams, and a team can participate
    /// in multiple events.
    /// </remarks>
    public class TeamEvent : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the team.
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the team associated with this event.
        /// </summary>
        public virtual Team Team { get; set; }

        /// <summary>
        /// Gets or sets the event associated with this team.
        /// </summary>
        public virtual Event Event { get; set; }
    }
}
