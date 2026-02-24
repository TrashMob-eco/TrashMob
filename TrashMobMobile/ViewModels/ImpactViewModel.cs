namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;

public partial class ImpactViewModel(
    IStatsRestService statsRestService,
    IEventAttendeeMetricsRestService eventAttendeeMetricsRestService,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    [ObservableProperty]
    private StatisticsViewModel personalStats = new();

    [ObservableProperty]
    private StatisticsViewModel communityStats = new();

    [ObservableProperty]
    private bool hasEventContributions;

    public ObservableCollection<UserEventMetricsSummary> EventContributions { get; set; } = [];

    public async Task Init()
    {
        await ExecuteAsync(async () =>
        {
            await Task.WhenAll(
                RefreshPersonalStats(),
                RefreshCommunityStats(),
                RefreshEventContributions());
        }, "Failed to load impact stats. Please try again.");
    }

    private async Task RefreshPersonalStats()
    {
        var stats = await statsRestService.GetUserStatsAsync(userManager.CurrentUser.Id);

        PersonalStats = new StatisticsViewModel
        {
            TotalEvents = stats.TotalEvents,
            TotalBags = stats.TotalBags,
            TotalHours = stats.TotalHours,
            TotalWeightInPounds = stats.TotalWeightInPounds,
            TotalLitterReportsSubmitted = stats.TotalLitterReportsSubmitted,
            TotalLitterReportsClosed = stats.TotalLitterReportsClosed,
        };
    }

    [RelayCommand]
    private async Task ViewLeaderboards()
    {
        await Shell.Current.GoToAsync(nameof(LeaderboardsPage));
    }

    [RelayCommand]
    private async Task ViewAchievements()
    {
        await Shell.Current.GoToAsync(nameof(AchievementsPage));
    }

    private async Task RefreshCommunityStats()
    {
        var stats = await statsRestService.GetStatsAsync();

        CommunityStats = new StatisticsViewModel
        {
            TotalAttendees = stats.TotalParticipants,
            TotalEvents = stats.TotalEvents,
            TotalBags = stats.TotalBags,
            TotalHours = stats.TotalHours,
            TotalWeightInPounds = stats.TotalWeightInPounds,
            TotalLitterReportsSubmitted = stats.TotalLitterReportsSubmitted,
            TotalLitterReportsClosed = stats.TotalLitterReportsClosed,
        };
    }

    private async Task RefreshEventContributions()
    {
        var impact = await eventAttendeeMetricsRestService.GetUserImpactAsync(userManager.CurrentUser.Id);

        EventContributions.Clear();

        foreach (var item in impact.EventBreakdown.OrderByDescending(e => e.EventDate).Take(10))
        {
            EventContributions.Add(item);
        }

        HasEventContributions = EventContributions.Count > 0;
    }

    [RelayCommand]
    private async Task ViewEventContribution(UserEventMetricsSummary? contribution)
    {
        if (contribution == null)
        {
            return;
        }

        await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={contribution.EventId}");
    }
}
