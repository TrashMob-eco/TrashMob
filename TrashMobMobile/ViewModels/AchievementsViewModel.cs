namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Services;

public partial class AchievementsViewModel(
    IAchievementManager achievementManager,
    INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly IAchievementManager achievementManager = achievementManager;

    [ObservableProperty]
    private string progressDisplay = string.Empty;

    [ObservableProperty]
    private string totalPointsDisplay = string.Empty;

    [ObservableProperty]
    private bool areAchievementsFound;

    [ObservableProperty]
    private bool areNoAchievementsFound = true;

    public ObservableCollection<AchievementViewModel> Achievements { get; } = [];

    public async Task Init()
    {
        await ExecuteAsync(async () =>
        {
            var response = await achievementManager.GetMyAchievementsAsync();

            Achievements.Clear();

            foreach (var achievement in response.Achievements.OrderByDescending(a => a.IsEarned)
                         .ThenByDescending(a => a.EarnedDate)
                         .ThenBy(a => a.DisplayName))
            {
                Achievements.Add(new AchievementViewModel
                {
                    Id = achievement.Id,
                    DisplayName = achievement.DisplayName,
                    Description = achievement.Description,
                    IconUrl = achievement.IconUrl,
                    Points = achievement.Points,
                    IsEarned = achievement.IsEarned,
                    EarnedDate = achievement.EarnedDate,
                    Category = achievement.Category,
                });
            }

            AreAchievementsFound = Achievements.Count > 0;
            AreNoAchievementsFound = !AreAchievementsFound;
            ProgressDisplay = $"{response.EarnedCount} of {response.TotalCount} earned";
            TotalPointsDisplay = $"{response.TotalPoints} points";

            var unreadIds = response.Achievements
                .Where(a => a.IsEarned)
                .Select(a => a.Id)
                .ToList();

            if (unreadIds.Count > 0)
            {
                await achievementManager.MarkAsReadAsync(unreadIds);
            }
        }, "Failed to load achievements. Please try again.");
    }
}
