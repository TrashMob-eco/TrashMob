namespace TrashMobMobileApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Models;

    public class WaiverManager : IWaiverManager
    {
        private readonly IWaiverRestService waiverRestService;

        public WaiverManager(IWaiverRestService service)
        {
            waiverRestService = service;
        }

        public Task<EnvelopeResponse> GetWaiverEnvelopeAsync(EnvelopeRequest envelopeRequest, CancellationToken cancellationToken = default)
        {
            return waiverRestService.GetWaiverEnvelopeAsync(envelopeRequest, cancellationToken);
        }
    }
}

