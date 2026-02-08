namespace TrashMobMobile.Models;

public class AchievementDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string IconUrl { get; set; } = string.Empty;

    public int Points { get; set; }

    public bool IsEarned { get; set; }

    public DateTimeOffset? EarnedDate { get; set; }
}
