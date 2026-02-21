namespace TrashMob.Shared.Poco
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an achievement type with earned status for a user.
    /// </summary>
    public class AchievementDto
    {
        /// <summary>
        /// Gets or sets the achievement type ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the achievement.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name of the achievement.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description of the achievement.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the category of the achievement.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the URL of the achievement's icon.
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Gets or sets the points awarded for this achievement.
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets whether this achievement has been earned by the user.
        /// </summary>
        public bool IsEarned { get; set; }

        /// <summary>
        /// Gets or sets the date when the achievement was earned. Null if not earned.
        /// </summary>
        public DateTimeOffset? EarnedDate { get; set; }
    }

    /// <summary>
    /// Represents the response for a user's achievements.
    /// </summary>
    public class UserAchievementsResponse
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the total points earned from all achievements.
        /// </summary>
        public int TotalPoints { get; set; }

        /// <summary>
        /// Gets or sets the count of achievements earned.
        /// </summary>
        public int EarnedCount { get; set; }

        /// <summary>
        /// Gets or sets the total count of available achievements.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the list of achievements with earned status.
        /// </summary>
        public List<AchievementDto> Achievements { get; set; } = new();
    }

    /// <summary>
    /// Represents a newly earned achievement notification.
    /// </summary>
    public class NewAchievementNotification
    {
        /// <summary>
        /// Gets or sets the achievement that was earned.
        /// </summary>
        public AchievementDto Achievement { get; set; }

        /// <summary>
        /// Gets or sets when the achievement was earned.
        /// </summary>
        public DateTimeOffset EarnedDate { get; set; }
    }
}
