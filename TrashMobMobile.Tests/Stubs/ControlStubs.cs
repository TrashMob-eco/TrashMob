namespace TrashMobMobile.Controls;

/// <summary>
/// Stub types for Controls referenced by ViewModels.
/// These satisfy compilation when linked ViewModel source files reference Controls.
/// The actual implementations live in TrashMobMobile/Controls/ with XAML code-behind.
/// </summary>

public partial class ListSelectorPopup : ContentView
{
    public ListSelectorPopup(string title, IEnumerable<string> items) { }
}

public partial class ConfirmPopup : ContentView
{
    public const string Confirmed = "Confirmed";

    public ConfirmPopup(string title, string message, string confirmText) { }
}

public partial class PhotoSourcePopup : ContentView
{
    public const string TakePhoto = "TakePhoto";
    public const string ChooseGallery = "ChooseGallery";

    public PhotoSourcePopup() { }
}

public partial class PhotoTypePopup : ContentView
{
    public const string Before = "Before";
    public const string During = "During";
    public const string After = "After";

    public PhotoTypePopup() { }
}

public partial class PrivacyPopup : ContentView
{
    public const string Private = "Private";
    public const string EventOnly = "EventOnly";
    public const string Public = "Public";

    public PrivacyPopup() { }
}

public partial class SelectDependentsPopup : ContentView
{
    public SelectDependentsPopup(List<TrashMob.Models.Dependent> dependents) { }
}

public partial class LogPickupPopup : ContentView
{
    public LogPickupPopup(int? currentBags = null, decimal? currentWeight = null,
        int? currentWeightUnitId = null, string? currentNotes = null) { }
}

public class LogPickupResult
{
    public int? BagsCollected { get; set; }
    public decimal? WeightCollected { get; set; }
    public int? WeightUnitId { get; set; }
    public string? Notes { get; set; }
}

public partial class TrimRoutePopup : ContentView
{
    public TrimRoutePopup(DateTimeOffset routeStartTime, DateTimeOffset routeEndTime) { }
}

public partial class LogImpactPopup : ContentView
{
    public LogImpactPopup(TrashMob.Models.EventAttendeeMetrics? existing = null) { }

    public class LogImpactResult
    {
        public int BagsCollected { get; set; }
        public decimal PickedWeight { get; set; }
        public int PickedWeightUnitId { get; set; }
        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }
    }
}

public partial class ReviewMetricsPopup : ContentView
{
    public const string ApprovedResult = "Approved";
    public const string RejectedPrefix = "Rejected:";

    public ReviewMetricsPopup(TrashMob.Models.EventAttendeeMetrics metrics) { }
}
