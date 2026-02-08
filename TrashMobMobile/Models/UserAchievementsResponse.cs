namespace TrashMobMobile.Models;

public class UserAchievementsResponse
{
    public Guid UserId { get; set; }

    public int TotalPoints { get; set; }

    public int EarnedCount { get; set; }

    public int TotalCount { get; set; }

    public List<AchievementDto> Achievements { get; set; } = [];
}
