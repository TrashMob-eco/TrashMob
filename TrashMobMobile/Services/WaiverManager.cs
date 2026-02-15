namespace TrashMobMobile.Services;

using TrashMob.Models;
using TrashMobMobile.Models;

public class WaiverManager(IWaiverRestService waiverRestService) : IWaiverManager
{
    public async Task<bool> HasUserSignedAllRequiredWaiversAsync(CancellationToken cancellationToken = default)
    {
        var required = await waiverRestService.GetRequiredWaiversAsync(null, cancellationToken);
        return required.Count == 0;
    }

    public Task<List<WaiverVersion>> GetRequiredWaiversAsync(CancellationToken cancellationToken = default)
    {
        return waiverRestService.GetRequiredWaiversAsync(null, cancellationToken);
    }

    public Task<List<UserWaiver>> GetMyWaiversAsync(CancellationToken cancellationToken = default)
    {
        return waiverRestService.GetMyWaiversAsync(cancellationToken);
    }

    public Task<UserWaiver> AcceptWaiverAsync(AcceptWaiverApiRequest request, CancellationToken cancellationToken = default)
    {
        return waiverRestService.AcceptWaiverAsync(request, cancellationToken);
    }
}
