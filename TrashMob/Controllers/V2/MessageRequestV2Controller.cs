namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for sending message requests (admin broadcast messages).
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/messagerequest")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class MessageRequestV2Controller(
        IMessageRequestManager messageRequestManager,
        ILogger<MessageRequestV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Sends a message request (broadcast message to users).
        /// </summary>
        /// <param name="messageRequestDto">The message request to send.</param>
        /// <returns>Ok on success.</returns>
        /// <response code="200">Message request sent successfully.</response>
        /// <response code="400">Invalid request data.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendMessageRequest([FromBody] MessageRequestDto messageRequestDto)
        {
            if (messageRequestDto == null)
            {
                return Problem("Message request cannot be null.", statusCode: StatusCodes.Status400BadRequest);
            }

            logger.LogInformation("V2 SendMessageRequest: name={Name}", messageRequestDto.Name);

            var entity = messageRequestDto.ToEntity();

            await messageRequestManager.SendMessageRequestAsync(entity);

            return Ok();
        }
    }
}
