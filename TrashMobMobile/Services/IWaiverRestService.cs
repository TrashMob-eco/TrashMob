namespace TrashMobMobile.Services;

using TrashMob.Models;
using TrashMobMobile.Models;

public interface IWaiverRestService
{
    Task<List<WaiverVersion>> GetRequiredWaiversAsync(Guid? communityId = null, CancellationToken cancellationToken = default);

    Task<List<UserWaiver>> GetMyWaiversAsync(CancellationToken cancellationToken = default);

    Task<UserWaiver> AcceptWaiverAsync(AcceptWaiverApiRequest request, CancellationToken cancellationToken = default);
}
