#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents an IFTTT (If This Then That) trigger configuration for a user.
    /// </summary>
    public class IftttTrigger : BaseModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the trigger.
        /// </summary>
        public string TriggerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who owns this trigger.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the trigger field configuration in JSON format.
        /// </summary>
        public string TriggerFields { get; set; }

        /// <summary>
        /// Gets or sets the source identifier for the IFTTT integration.
        /// </summary>
        public string IftttSource { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of items to return for this trigger.
        /// </summary>
        public int Limit { get; set; }
    }
}