namespace TrashMobMobile.Services;

using TrashMobMobile.Config;
using TrashMobMobile.Extensions;

public class WaiverManager : IWaiverManager
{
    private const string TrashMobWaiverName = "trashmob";
 
    private readonly IWaiverRestService waiverRestService;

    public WaiverManager(IWaiverRestService service)
    {
        waiverRestService = service;
    }

    public async Task<bool> HasUserSignedTrashMobWaiverAsync(CancellationToken cancellationToken = default)
    {
        var waiver = await waiverRestService.GetWaiver(TrashMobWaiverName, cancellationToken);
        return App.CurrentUser.HasUserSignedWaiver(waiver, Settings.CurrentTrashMobWaiverVersion);
    }
}