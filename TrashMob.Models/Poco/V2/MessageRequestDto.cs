#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a message request (broadcast message to users).
    /// </summary>
    public class MessageRequestDto
    {
        /// <summary>Gets or sets the unique identifier.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the message name/subject.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the message content.</summary>
        public string Message { get; set; } = string.Empty;
    }
}
