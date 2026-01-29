#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a message request or communication template.
    /// </summary>
    public class MessageRequest : KeyedModel
    {
        /// <summary>
        /// Gets or sets the name or subject of the message request.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        public string Message { get; set; }
    }
}