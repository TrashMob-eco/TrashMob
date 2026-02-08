namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Services;

public partial class LeaderboardsViewModel(
    ILeaderboardManager leaderboardManager,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly ILeaderboardManager leaderboardManager = leaderboardManager;
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    private bool showUsers = true;

    [ObservableProperty]
    private bool showTeams;

    [ObservableProperty]
    private string selectedType = "Events";

    [ObservableProperty]
    private string selectedTimeRange = "Month";

    [ObservableProperty]
    private string myRankDisplay = string.Empty;

    [ObservableProperty]
    private string myScoreDisplay = string.Empty;

    [ObservableProperty]
    private bool isMyRankVisible;

    [ObservableProperty]
    private bool areEntriesFound;

    [ObservableProperty]
    private bool areNoEntriesFound = true;

    [ObservableProperty]
    private string totalEntriesDisplay = string.Empty;

    public ObservableCollection<LeaderboardEntryViewModel> Entries { get; } = [];

    public List<string> LeaderboardTypes { get; } = ["Events", "Bags", "Hours"];

    public List<string> TimeRanges { get; } = ["Week", "Month", "Year", "AllTime"];

    public async Task Init()
    {
        await LoadLeaderboard();
    }

    [RelayCommand]
    private async Task ShowUserLeaderboard()
    {
        ShowUsers = true;
        ShowTeams = false;
        await LoadLeaderboard();
    }

    [RelayCommand]
    private async Task ShowTeamLeaderboard()
    {
        ShowUsers = false;
        ShowTeams = true;
        await LoadLeaderboard();
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadLeaderboard();
    }

    partial void OnSelectedTypeChanged(string value)
    {
        _ = LoadLeaderboard();
    }

    partial void OnSelectedTimeRangeChanged(string value)
    {
        _ = LoadLeaderboard();
    }

    private async Task LoadLeaderboard()
    {
        await ExecuteAsync(async () =>
        {
            var response = ShowTeams
                ? await leaderboardManager.GetTeamLeaderboardAsync(SelectedType, SelectedTimeRange)
                : await leaderboardManager.GetLeaderboardAsync(SelectedType, SelectedTimeRange);

            var currentUserId = userManager.CurrentUser.Id;

            Entries.Clear();

            foreach (var entry in response.Entries)
            {
                Entries.Add(new LeaderboardEntryViewModel
                {
                    Rank = entry.Rank,
                    EntityName = entry.EntityName,
                    FormattedScore = entry.FormattedScore,
                    IsCurrentUser = !ShowTeams && entry.EntityId == currentUserId,
                });
            }

            AreEntriesFound = Entries.Count > 0;
            AreNoEntriesFound = !AreEntriesFound;
            TotalEntriesDisplay = $"{response.TotalEntries} total";

            if (ShowUsers)
            {
                await LoadMyRank();
            }
            else
            {
                IsMyRankVisible = false;
            }
        }, "Failed to load leaderboard. Please try again.");
    }

    private async Task LoadMyRank()
    {
        var rank = await leaderboardManager.GetMyRankAsync(SelectedType, SelectedTimeRange);

        if (rank.Rank.HasValue)
        {
            MyRankDisplay = $"Your rank: #{rank.Rank.Value} of {rank.TotalRanked}";
            MyScoreDisplay = rank.FormattedScore;
            IsMyRankVisible = true;
        }
        else
        {
            MyRankDisplay = rank.IsEligible ? "Not yet ranked" : "Not eligible";
            MyScoreDisplay = string.Empty;
            IsMyRankVisible = true;
        }
    }
}
