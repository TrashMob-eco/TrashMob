namespace TrashMob.Shared.Poco
{
    using System;

    /// <summary>
    /// Represents the waiver status for an event attendee.
    /// </summary>
    public class AttendeeWaiverStatus
    {
        /// <summary>
        /// Gets or sets the user ID of the attendee.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the attendee.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets whether the attendee has a valid waiver for the event.
        /// </summary>
        public bool HasValidWaiver { get; set; }
    }
}
