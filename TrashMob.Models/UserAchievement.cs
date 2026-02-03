#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents an achievement earned by a user.
    /// </summary>
    /// <remarks>
    /// This entity tracks when a user earns a specific achievement and whether they have
    /// been notified about it. Each user can only earn each achievement type once.
    /// </remarks>
    public class UserAchievement : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the user who earned this achievement.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the achievement type earned.
        /// </summary>
        public int AchievementTypeId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the achievement was earned.
        /// </summary>
        public DateTimeOffset EarnedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a notification has been sent for this achievement.
        /// </summary>
        public bool NotificationSent { get; set; }

        /// <summary>
        /// Gets or sets the user who earned this achievement.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the achievement type that was earned.
        /// </summary>
        public virtual AchievementType AchievementType { get; set; }
    }
}
