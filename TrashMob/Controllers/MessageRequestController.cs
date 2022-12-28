namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/messagerequest")]
    public class MessageRequestController : SecureController
    {
        private readonly IMessageRequestManager messageRequestManager;

        public MessageRequestController(IMessageRequestManager messageRequestManager) : base()
        {
            this.messageRequestManager = messageRequestManager;
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> SendMessageRequest(MessageRequest messageRequest)
        {
            await messageRequestManager.SendMessageRequestAsync(messageRequest).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(SendMessageRequest));

            return Ok();
        }
    }
}
