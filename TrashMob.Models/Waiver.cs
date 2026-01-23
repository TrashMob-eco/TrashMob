namespace TrashMob.Models
{
    /// <summary>
    /// Represents a waiver that users may need to agree to before participating in events.
    /// </summary>
    public class Waiver : KeyedModel
    {
        /// <summary>
        /// Gets or sets the name of the waiver.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the waiver is enabled and required.
        /// </summary>
        public bool IsWaiverEnabled { get; set; } = true;
    }
}