#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 DTO for scheduling a newsletter.
    /// </summary>
    public class ScheduleNewsletterDto
    {
        /// <summary>Gets or sets the scheduled send date and time.</summary>
        public DateTimeOffset ScheduledDate { get; set; }
    }
}
