namespace TrashMobMobile.ViewModels;

public class AchievementViewModel
{
    // Category-based icons matching the website (lucide-react equivalents)
    // Website: Participation → Target, Impact → Star, Special → Award/Medal
    private static readonly Dictionary<string, string> CategoryIconKeys = new()
    {
        ["Participation"] = "IconTarget",
        ["Impact"] = "IconStar",
        ["Special"] = "IconMedal",
    };

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

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

    public ImageSource? IconImageSource
    {
        get
        {
            if (!string.IsNullOrEmpty(IconUrl))
            {
                return ImageSource.FromUri(new Uri(IconUrl));
            }

            // Use earned/locked icon matching the website pattern:
            // Earned → CheckCircle, Not earned → Lock
            var iconKey = IsEarned ? "IconCheckCircle" : "IconLock";

            if (Application.Current?.Resources.TryGetValue(iconKey, out var glyph) == true
                && glyph is string glyphStr)
            {
                var colorKey = IsEarned ? "Primary" : "MutedForegroundLight";
                Color iconColor = Colors.Grey;
                if (Application.Current.Resources.TryGetValue(colorKey, out var colorObj)
                    && colorObj is Color color)
                {
                    iconColor = color;
                }

                return new FontImageSource
                {
                    FontFamily = "GoogleMaterialIcons",
                    Glyph = glyphStr,
                    Color = iconColor,
                    Size = 48,
                };
            }

            return null;
        }
    }
}
