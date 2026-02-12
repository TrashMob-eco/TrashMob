namespace TrashMobMobile.Services;

using TrashMobMobile.Config;
using TrashMobMobile.Extensions;

public class WaiverManager : IWaiverManager
{
    private const string TrashMobWaiverName = "trashmob";

    private readonly IWaiverRestService waiverRestService;
    private readonly IUserManager userManager;

    public WaiverManager(IWaiverRestService service, IUserManager userManager)
    {
        waiverRestService = service;
        this.userManager = userManager;
    }

    public async Task<bool> HasUserSignedTrashMobWaiverAsync(CancellationToken cancellationToken = default)
    {
        var waiver = await waiverRestService.GetWaiver(TrashMobWaiverName, cancellationToken);
        return userManager.CurrentUser.HasUserSignedWaiver(waiver, Settings.CurrentTrashMobWaiverVersion);
    }
}
