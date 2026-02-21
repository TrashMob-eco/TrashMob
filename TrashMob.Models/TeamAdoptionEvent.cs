#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents the link between a cleanup event and a team adoption for compliance tracking.
    /// </summary>
    /// <remarks>
    /// When a team hosts a cleanup event for an adopted area, the event can be linked
    /// to the adoption to track compliance with cleanup frequency requirements.
    /// Events are optionally linked during event creation if the team has active adoptions.
    /// </remarks>
    public class TeamAdoptionEvent : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the team adoption.
        /// </summary>
        public Guid TeamAdoptionId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the cleanup event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets optional notes about why this event is linked to the adoption.
        /// </summary>
        public string Notes { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Gets or sets the team adoption this event is linked to.
        /// </summary>
        public virtual TeamAdoption TeamAdoption { get; set; }

        /// <summary>
        /// Gets or sets the cleanup event.
        /// </summary>
        public virtual Event Event { get; set; }

        #endregion
    }
}
