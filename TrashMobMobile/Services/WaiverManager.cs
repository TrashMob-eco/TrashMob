namespace TrashMobMobile.Services;

using TrashMobMobile.Config;
using TrashMobMobile.Extensions;
using TrashMobMobile.Models;

public class WaiverManager : IWaiverManager
{
    private const string TrashMobWaiverName = "trashmob";
    private readonly IDocusignRestService docusignRestService;

    private readonly IWaiverRestService waiverRestService;

    public WaiverManager(IWaiverRestService service, IDocusignRestService docusignRestService)
    {
        waiverRestService = service;
        this.docusignRestService = docusignRestService;
    }

    public Task<EnvelopeResponse> GetWaiverEnvelopeAsync(EnvelopeRequest envelopeRequest,
        CancellationToken cancellationToken = default)
    {
        return docusignRestService.GetWaiverEnvelopeAsync(envelopeRequest, cancellationToken);
    }

    public async Task<bool> HasUserSignedTrashMobWaiverAsync(CancellationToken cancellationToken = default)
    {
        var waiver = await waiverRestService.GetWaiver(TrashMobWaiverName, cancellationToken);
        return App.CurrentUser.HasUserSignedWaiver(waiver, Settings.CurrentTrashMobWaiverVersion);
    }
}