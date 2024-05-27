namespace TrashMob.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/messagerequest")]
    public class MessageRequestController : SecureController
    {
        private readonly IMessageRequestManager messageRequestManager;

        public MessageRequestController(IMessageRequestManager messageRequestManager)
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