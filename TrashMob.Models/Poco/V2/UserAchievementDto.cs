#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of an individual achievement with the user's earned status.
    /// </summary>
    public class UserAchievementDto
    {
        /// <summary>
        /// Gets or sets the achievement type identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the machine name of the achievement.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user-facing display name.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the achievement.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category (e.g., Participation, Impact, Special).
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the achievement's badge icon.
        /// </summary>
        public string IconUrl { get; set; } = string.Empty;

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
}
