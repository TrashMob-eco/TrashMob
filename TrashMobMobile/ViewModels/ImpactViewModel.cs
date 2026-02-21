namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Services;

public partial class ImpactViewModel(
    IStatsRestService statsRestService,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    [ObservableProperty]
    private StatisticsViewModel personalStats = new();

    [ObservableProperty]
    private StatisticsViewModel communityStats = new();

    public async Task Init()
    {
        await ExecuteAsync(async () =>
        {
            await Task.WhenAll(
                RefreshPersonalStats(),
                RefreshCommunityStats());
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
}
