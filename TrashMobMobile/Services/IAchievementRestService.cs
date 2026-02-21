namespace TrashMobMobile.Services;

using TrashMobMobile.Models;

public interface IAchievementRestService
{
    Task<UserAchievementsResponse> GetMyAchievementsAsync(CancellationToken cancellationToken = default);

    Task MarkAsReadAsync(IEnumerable<int> achievementTypeIds, CancellationToken cancellationToken = default);
}
