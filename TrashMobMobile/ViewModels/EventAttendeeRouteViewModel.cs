namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using TrashMob.Models;
using TrashMob.Models.Poco;

public partial class EventAttendeeRouteViewModel : ObservableObject
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public Guid UserId { get; set; }

    public string DistanceDisplay { get; set; } = string.Empty;

    public string DurationDisplay { get; set; } = string.Empty;

    [ObservableProperty]
    private string privacyLevel = "EventOnly";

    public string PrivacyDisplay => PrivacyLevel switch
    {
        "Private" => "Private",
        "Public" => "Public",
        _ => "Event Only",
    };

    [ObservableProperty]
    private string bagsDisplay = string.Empty;

    [ObservableProperty]
    private string weightDisplay = string.Empty;

    public int? BagsCollected { get; set; }

    public decimal? WeightCollected { get; set; }

    public int? WeightUnitId { get; set; }

    public string? Notes { get; set; }

    public bool IsOwnRoute { get; set; }

    public bool CanDelete => IsOwnRoute;

    public bool CanChangePrivacy => IsOwnRoute;

    public bool HasMetrics => !string.IsNullOrEmpty(BagsDisplay) || !string.IsNullOrEmpty(WeightDisplay);

    public List<SortableLocation> Locations { get; set; } = [];

    public static EventAttendeeRouteViewModel FromRoute(DisplayEventAttendeeRoute route, Guid currentUserId)
    {
        var weightUnitLabel = route.WeightUnitId == (int)WeightUnitEnum.Kilogram ? "kg" : "lbs";

        return new EventAttendeeRouteViewModel
        {
            Id = route.Id,
            EventId = route.EventId,
            UserId = route.UserId,
            PrivacyLevel = route.PrivacyLevel,
            DistanceDisplay = FormatDistance(route.TotalDistanceMeters),
            DurationDisplay = FormatDuration(route.DurationMinutes),
            BagsCollected = route.BagsCollected,
            WeightCollected = route.WeightCollected,
            WeightUnitId = route.WeightUnitId,
            Notes = route.Notes,
            BagsDisplay = route.BagsCollected.HasValue ? $"{route.BagsCollected} bags" : string.Empty,
            WeightDisplay = route.WeightCollected.HasValue ? $"{route.WeightCollected:F1} {weightUnitLabel}" : string.Empty,
            IsOwnRoute = route.UserId == currentUserId,
            Locations = route.Locations,
        };
    }

    private static string FormatDistance(int meters)
    {
        if (meters >= 1000)
        {
            return $"{meters / 1000.0:F1} km";
        }

        return $"{meters} m";
    }

    private static string FormatDuration(int minutes)
    {
        if (minutes >= 60)
        {
            var hours = minutes / 60;
            var mins = minutes % 60;
            return mins > 0 ? $"{hours} hr {mins} min" : $"{hours} hr";
        }

        return $"{minutes} min";
    }
}
