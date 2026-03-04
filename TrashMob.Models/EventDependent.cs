#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a dependent minor's registration for a specific event.
    /// </summary>
    /// <remarks>
    /// Links a dependent to an event with a reference to the covering waiver.
    /// The parent must be a registered attendee for the event.
    /// </remarks>
    public class EventDependent : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the dependent attending the event.
        /// </summary>
        public Guid DependentId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the parent/guardian user (for easy querying).
        /// </summary>
        public Guid ParentUserId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the waiver covering this dependent for this event.
        /// </summary>
        public Guid DependentWaiverId { get; set; }

        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// Gets or sets the dependent.
        /// </summary>
        public virtual Dependent Dependent { get; set; }

        /// <summary>
        /// Gets or sets the parent/guardian user.
        /// </summary>
        public virtual User ParentUser { get; set; }

        /// <summary>
        /// Gets or sets the waiver covering this dependent.
        /// </summary>
        public virtual DependentWaiver DependentWaiver { get; set; }
    }
}
