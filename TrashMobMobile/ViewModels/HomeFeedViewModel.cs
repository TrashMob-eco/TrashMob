namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class HomeFeedViewModel(
    IMobEventManager mobEventManager,
    ILitterReportManager litterReportManager,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

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
        await ExecuteAsync(async () =>
        {
            var user = userManager.CurrentUser;
            WelcomeMessage = $"Welcome, {user.UserName}!";

            await Task.WhenAll(
                RefreshUpcomingEvents(),
                RefreshNearbyLitterReports());
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
        var filter = new LitterReportFilter
        {
            StartDate = DateTimeOffset.UtcNow.AddDays(-7),
            EndDate = DateTimeOffset.UtcNow,
        };

        var reports = await litterReportManager.GetLitterReportsAsync(filter, ImageSizeEnum.Thumb, true);
        var user = userManager.CurrentUser;
        var maxDistanceKm = GetMaxDistanceKm(user);

        NearbyLitterReports.Clear();

        var filteredReports = reports
            .Where(r => r.LitterReportStatusId != (int)LitterReportStatusEnum.Cancelled)
            .OrderByDescending(r => r.CreatedDate)
            .AsEnumerable();

        if (maxDistanceKm.HasValue && user.Latitude.HasValue && user.Longitude.HasValue)
        {
            filteredReports = filteredReports.Where(r =>
            {
                var firstImage = r.LitterImages?.FirstOrDefault();
                if (firstImage?.Latitude == null || firstImage?.Longitude == null)
                {
                    return true; // Include reports without location data
                }

                return DistanceInKm(user.Latitude.Value, user.Longitude.Value, firstImage.Latitude.Value, firstImage.Longitude.Value) <= maxDistanceKm.Value;
            });
        }

        foreach (var report in filteredReports.Take(3))
        {
            NearbyLitterReports.Add(report.ToLitterReportViewModel(NotificationService));
        }

        AreLitterReportsFound = NearbyLitterReports.Count > 0;
        AreNoLitterReportsFound = !AreLitterReportsFound;
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
