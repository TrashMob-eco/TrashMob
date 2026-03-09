#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a newly earned achievement notification.
    /// </summary>
    public class AchievementNotificationDto
    {
        /// <summary>
        /// Gets or sets the achievement that was earned.
        /// </summary>
        public UserAchievementDto Achievement { get; set; } = new();

        /// <summary>
        /// Gets or sets when the achievement was earned.
        /// </summary>
        public DateTimeOffset EarnedDate { get; set; }
    }
}
