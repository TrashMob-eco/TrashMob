#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a user's attendance registration for an event.
    /// </summary>
    public class EventAttendee : BaseModel
    {
        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the attending user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user signed up for the event.
        /// </summary>
        public DateTimeOffset SignUpDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the attendance was canceled, if applicable.
        /// </summary>
        public DateTimeOffset? CanceledDate { get; set; }

        /// <summary>
        /// Gets or sets whether this attendee is an event lead with management permissions.
        /// Maximum 5 co-leads per event enforced at API level.
        /// </summary>
        public bool IsEventLead { get; set; }

        /// <summary>
        /// Gets or sets the event being attended.
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// Gets or sets the user who is attending.
        /// </summary>
        public virtual User User { get; set; }
    }
}