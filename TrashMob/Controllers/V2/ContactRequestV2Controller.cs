namespace TrashMob.Controllers.V2
{
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for contact request submissions.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/contactrequest")]
    public class ContactRequestV2Controller(
        IKeyedManager<ContactRequest> contactRequestManager,
        ILogger<ContactRequestV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Submits a new contact request.
        /// </summary>
        /// <param name="dto">The contact request details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>201 Created on success.</returns>
        /// <response code="201">Contact request submitted.</response>
        /// <response code="400">Invalid request.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(
            [FromBody] ContactRequestDto dto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddContactRequest requested");

            var entity = dto.ToEntity();
            await contactRequestManager.AddAsync(entity, cancellationToken);

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
