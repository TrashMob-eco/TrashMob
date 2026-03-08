#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of an event attendee with basic user info.
    /// </summary>
    public class EventAttendeeDto
    {
        /// <summary>
        /// Gets or sets the identifier of the attending user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the display username of the attendee.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the given (first) name of the attendee.
        /// </summary>
        public string GivenName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the attendee's profile photo.
        /// </summary>
        public string ProfilePhotoUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the user signed up.
        /// </summary>
        public DateTimeOffset SignUpDate { get; set; }

        /// <summary>
        /// Gets or sets whether this attendee is an event lead.
        /// </summary>
        public bool IsEventLead { get; set; }
    }
}
