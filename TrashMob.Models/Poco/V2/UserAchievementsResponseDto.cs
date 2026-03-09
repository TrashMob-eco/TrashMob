#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// V2 API representation of a user's complete achievements summary.
    /// </summary>
    public class UserAchievementsResponseDto
    {
        /// <summary>
        /// Gets or sets the user identifier.
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
        public IReadOnlyList<UserAchievementDto> Achievements { get; set; } = [];
    }
}
