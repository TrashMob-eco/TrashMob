namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Config;
    using TrashMobMobileApp.Extensions;
    using TrashMobMobileApp.Models;

    public class WaiverManager : IWaiverManager
    {
        const string TrashMobWaiverName = "trashmob";

        private readonly IWaiverRestService waiverRestService;
        private readonly IDocusignRestService docusignRestService;
        private readonly IOptions<Settings> settings;

        public WaiverManager(IWaiverRestService service, IDocusignRestService docusignRestService, IOptions<Settings> settings)
        {
            waiverRestService = service;
            this.docusignRestService = docusignRestService;
            this.settings = settings;
        }

        public Task<EnvelopeResponse> GetWaiverEnvelopeAsync(EnvelopeRequest envelopeRequest, CancellationToken cancellationToken = default)
        {
            return docusignRestService.GetWaiverEnvelopeAsync(envelopeRequest, cancellationToken);
        }

        public async Task<bool> HasUserSignedTrashMobWaiverAsync(CancellationToken cancellationToken = default)
        {
            var waiver = await waiverRestService.GetWaiver(TrashMobWaiverName, cancellationToken);
            return App.CurrentUser.HasUserSignedWaiver(waiver, settings.Value.CurrentTrashMobWaiverVersion);
        }
    }
}

