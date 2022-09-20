namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using System.Threading.Tasks;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [Route("api/messagerequest")]
    public class MessageRequestController : BaseController
    {
        private readonly IMessageRequestManager messageRequestManager;

        public MessageRequestController(IMessageRequestManager messageRequestManager, 
                                        TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.messageRequestManager = messageRequestManager;
        }

        [HttpPost]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> SendMessageRequest(MessageRequest messageRequest)
        {
            await messageRequestManager.SendMessageRequest(messageRequest).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(SendMessageRequest));

            return Ok();
        }
    }
}
