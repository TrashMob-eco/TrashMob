#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a type of achievement that users can earn.
    /// </summary>
    /// <remarks>
    /// Achievement types define the criteria for earning badges, such as attending a certain
    /// number of events, collecting bags, or creating events. The Criteria property stores
    /// JSON that defines the requirements for earning the achievement.
    /// </remarks>
    public class AchievementType : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AchievementType"/> class.
        /// </summary>
        public AchievementType()
        {
            UserAchievements = [];
        }

        /// <summary>
        /// Gets or sets the display name shown to users.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the category of the achievement (e.g., Participation, Impact, Special).
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the URL of the achievement's icon/badge image.
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Gets or sets the JSON criteria for earning this achievement.
        /// </summary>
        /// <example>
        /// {"eventsAttended": 10} or {"bagsCollected": 100}
        /// </example>
        public string Criteria { get; set; }

        /// <summary>
        /// Gets or sets the points awarded for earning this achievement.
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the collection of user achievements of this type.
        /// </summary>
        public virtual ICollection<UserAchievement> UserAchievements { get; set; }
    }
}
