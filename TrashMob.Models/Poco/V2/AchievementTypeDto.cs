#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// V2 API representation of an achievement type from the catalog.
    /// </summary>
    public class AchievementTypeDto
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
        /// Gets or sets the points awarded for earning this achievement.
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the display order for sorting in lists.
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
