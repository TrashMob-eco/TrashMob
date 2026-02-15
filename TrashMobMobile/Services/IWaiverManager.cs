namespace TrashMobMobile.Services;

using TrashMob.Models;
using TrashMobMobile.Models;

public interface IWaiverManager
{
    Task<bool> HasUserSignedAllRequiredWaiversAsync(CancellationToken cancellationToken = default);

    Task<List<WaiverVersion>> GetRequiredWaiversAsync(CancellationToken cancellationToken = default);

    Task<List<UserWaiver>> GetMyWaiversAsync(CancellationToken cancellationToken = default);

    Task<UserWaiver> AcceptWaiverAsync(AcceptWaiverApiRequest request, CancellationToken cancellationToken = default);
}
