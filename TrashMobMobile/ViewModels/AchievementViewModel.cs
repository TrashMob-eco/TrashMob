namespace TrashMobMobile.ViewModels;

public class AchievementViewModel
{
    public int Id { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string IconUrl { get; set; } = string.Empty;

    public int Points { get; set; }

    public bool IsEarned { get; set; }

    public DateTimeOffset? EarnedDate { get; set; }

    public string Category { get; set; } = string.Empty;

    public string EarnedDateDisplay => EarnedDate?.LocalDateTime.ToString("MMM d, yyyy") ?? string.Empty;

    public string PointsDisplay => $"{Points} pts";

    public double Opacity => IsEarned ? 1.0 : 0.4;
}
