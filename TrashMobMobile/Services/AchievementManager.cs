namespace TrashMobMobile.Services;

using TrashMobMobile.Models;

public class AchievementManager(IAchievementRestService achievementRestService) : IAchievementManager
{
    public Task<UserAchievementsResponse> GetMyAchievementsAsync(
        CancellationToken cancellationToken = default)
    {
        return achievementRestService.GetMyAchievementsAsync(cancellationToken);
    }

    public Task MarkAsReadAsync(IEnumerable<int> achievementTypeIds,
        CancellationToken cancellationToken = default)
    {
        return achievementRestService.MarkAsReadAsync(achievementTypeIds, cancellationToken);
    }
}
