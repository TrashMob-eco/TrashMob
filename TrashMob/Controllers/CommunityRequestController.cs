namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [Route("api/commuinityrequest")]
    public class CommunityRequestController : BaseController
    {
        private readonly ICommunityRequestManager communityRequestManager;
        private readonly IEmailManager emailManager;

        public CommunityRequestController(ICommunityRequestManager communityRequestManager, 
                                          TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.communityRequestManager = communityRequestManager;
        }

        [HttpPost]
        public async Task<IActionResult> SaveCommunityRequest(CommunityRequest communityRequest)
        {
            await communityRequestManager.AddCommunityRequest(communityRequest).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(SaveCommunityRequest));

            return Ok();
        }
    }
}
