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

    /// <summary>
    /// Controller for sending message requests. Admin only.
    /// </summary>
    [Route("api/messagerequest")]
    public class MessageRequestController(IMessageRequestManager messageRequestManager)
        : SecureController
    {

        /// <summary>
        /// Sends a message request. Admin only.
        /// </summary>
        /// <param name="messageRequest">The message request to send.</param>
        /// <remarks>Action result.</remarks>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> SendMessageRequest(MessageRequest messageRequest)
        {
            await messageRequestManager.SendMessageRequestAsync(messageRequest);

            TrackEvent(nameof(SendMessageRequest));

            return Ok();
        }
    }
}