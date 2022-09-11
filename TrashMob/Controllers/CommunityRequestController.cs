namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [Route("api/communityrequest")]
    public class CommunityRequestController : BaseController
    {
        private readonly ICommunityRequestManager communityRequestManager;

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
