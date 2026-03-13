namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class HomeFeedViewModel(
    IMobEventManager mobEventManager,
    ILitterReportManager litterReportManager,
    IStatsRestService statsRestService,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IStatsRestService statsRestService = statsRestService;
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    [ObservableProperty]
    private string userInitials = string.Empty;

    [ObservableProperty]
    private string locationSummary = string.Empty;

    [ObservableProperty]
    private StatisticsViewModel personalStats = new();

    [ObservableProperty]
    private bool areEventsFound;

    [ObservableProperty]
    private bool areNoEventsFound = true;

    [ObservableProperty]
    private bool areLitterReportsFound;

    [ObservableProperty]
    private bool areNoLitterReportsFound = true;

    public ObservableCollection<EventViewModel> UpcomingEvents { get; } = [];

    public ObservableCollection<LitterReportViewModel> NearbyLitterReports { get; } = [];

    public async Task Init()
    {
        if (IsBusy) return;

        await ExecuteAsync(async () =>
        {
            var user = userManager.CurrentUser;
            WelcomeMessage = $"Hi, {user.UserName}";
            UserInitials = GetInitials(user.UserName);
            LocationSummary = BuildLocationSummary(user);

            var tasks = new (string Name, Func<Task> Action)[]
            {
                ("RefreshUpcomingEvents", RefreshUpcomingEvents),
                ("RefreshNearbyLitterReports", RefreshNearbyLitterReports),
                ("RefreshPersonalStats", RefreshPersonalStats),
            };

            var errors = new List<string>();

            await Task.WhenAll(tasks.Select(async t =>
            {
                try
                {
                    await t.Action();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[HomeFeed] {t.Name} failed: {ex.GetType().Name}: {ex.Message}\n{ex}");
                    SentrySdk.CaptureException(ex);
                    errors.Add($"{t.Name}: {ex.Message}");
                }
            }));

            if (errors.Count > 0)
            {
                throw new InvalidOperationException(string.Join("; ", errors));
            }
        }, "Failed to load feed. Please try again.");
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await Init();
    }

    [RelayCommand]
    private async Task ViewEvent(EventViewModel? eventVm)
    {
        if (eventVm == null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ViewEventPage)}?EventId={eventVm.Id}");
    }

    [RelayCommand]
    private async Task BrowseTeams()
    {
        await Shell.Current.GoToAsync(nameof(BrowseTeamsPage));
    }

    [RelayCommand]
    private async Task BrowseCommunities()
    {
        await Shell.Current.GoToAsync(nameof(BrowseCommunitiesPage));
    }

    [RelayCommand]
    private async Task OpenProfile()
    {
        await Shell.Current.GoToAsync("//ProfilePage");
    }

    [RelayCommand]
    private async Task ViewImpact()
    {
        await Shell.Current.GoToAsync("//ImpactPage");
    }

    [RelayCommand]
    private async Task ViewLitterReport(LitterReportViewModel? reportVm)
    {
        if (reportVm == null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ViewLitterReportPage)}?LitterReportId={reportVm.Id}");
    }

    private async Task RefreshUpcomingEvents()
    {
        var filter = new EventFilter
        {
            StartDate = DateTimeOffset.UtcNow,
            EndDate = DateTimeOffset.UtcNow.AddMonths(3),
            PageIndex = 0,
            PageSize = 20,
        };

        var events = await mobEventManager.GetFilteredEventsAsync(filter);
        var user = userManager.CurrentUser;
        var maxDistanceKm = GetMaxDistanceKm(user);

        UpcomingEvents.Clear();

        var filteredEvents = events.Where(e => !e.IsCompleted());

        if (maxDistanceKm.HasValue && user.Latitude.HasValue && user.Longitude.HasValue)
        {
            filteredEvents = filteredEvents.Where(e =>
                !e.Latitude.HasValue || !e.Longitude.HasValue ||
                DistanceInKm(user.Latitude.Value, user.Longitude.Value, e.Latitude.Value, e.Longitude.Value) <= maxDistanceKm.Value);
        }

        foreach (var mobEvent in filteredEvents.OrderBy(e => e.EventDate).Take(10))
        {
            UpcomingEvents.Add(mobEvent.ToEventViewModel(user.Id));
        }

        AreEventsFound = UpcomingEvents.Count > 0;
        AreNoEventsFound = !AreEventsFound;
    }

    private async Task RefreshNearbyLitterReports()
    {
        // Fetch reports without images first (fast, reliable)
        var filter = new LitterReportFilter
        {
            StartDate = DateTimeOffset.UtcNow.AddDays(-7),
            EndDate = DateTimeOffset.UtcNow,
        };

        var reports = await litterReportManager.GetLitterReportsAsync(filter, ImageSizeEnum.Thumb, getImageUrls: false);
        var user = userManager.CurrentUser;

        NearbyLitterReports.Clear();

        var displayReports = reports
            .Where(r => r.LitterReportStatusId != (int)LitterReportStatusEnum.Cancelled)
            .OrderByDescending(r => r.CreatedDate)
            .Take(3)
            .ToList();

        // Fetch full details (with images) only for the reports we display
        foreach (var report in displayReports)
        {
            try
            {
                var fullReport = await litterReportManager.GetLitterReportAsync(report.Id, ImageSizeEnum.Thumb);
                NearbyLitterReports.Add(fullReport.ToLitterReportViewModel(NotificationService));
            }
            catch
            {
                // Fall back to report without thumbnail
                NearbyLitterReports.Add(report.ToLitterReportViewModel(NotificationService));
            }
        }

        AreLitterReportsFound = NearbyLitterReports.Count > 0;
        AreNoLitterReportsFound = !AreLitterReportsFound;
    }

    private async Task RefreshPersonalStats()
    {
        var user = userManager.CurrentUser;
        var stats = await statsRestService.GetUserStatsAsync(user.Id);

        PersonalStats = new StatisticsViewModel
        {
            TotalEvents = stats.TotalEvents,
            TotalBags = stats.TotalBags,
            TotalHours = stats.TotalHours,
            TotalWeightInPounds = stats.TotalWeightInPounds,
            TotalLitterReportsSubmitted = stats.TotalLitterReportsSubmitted,
        };
    }

    private static string GetInitials(string? userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return "?";
        }

        var parts = userName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant()
            : $"{parts[0][0]}".ToUpperInvariant();
    }

    private static string BuildLocationSummary(User user)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(user.City)) parts.Add(user.City);
        if (!string.IsNullOrEmpty(user.Region)) parts.Add(user.Region);
        return parts.Count > 0 ? string.Join(", ", parts) : string.Empty;
    }

    private static double? GetMaxDistanceKm(User user)
    {
        if (user.TravelLimitForLocalEvents <= 0 || !user.Latitude.HasValue || !user.Longitude.HasValue)
        {
            return null; // No filtering — show all
        }

        var distanceKm = user.PrefersMetric
            ? user.TravelLimitForLocalEvents
            : user.TravelLimitForLocalEvents * 1.60934;

        return distanceKm;
    }

    private static double DistanceInKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth radius in km
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
