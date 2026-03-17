namespace TrashMob.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for generating and sending volunteer participation reports.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/events/{eventId}/participation-report")]
    [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
    public class ParticipationReportV2Controller(
        IParticipationReportService reportService,
        ILogger<ParticipationReportV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;
        /// <summary>
        /// Sends a participation report email with PDF attachment to the requesting user.
        /// Only available for attendees with Approved or Adjusted metrics.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Report sent successfully.</response>
        /// <response code="400">Metrics not approved or other validation error.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestReport(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 RequestParticipationReport Event={EventId}, User={UserId}", eventId, UserId);

            var result = await reportService.SendReportAsync(eventId, UserId, cancellationToken);

            if (!result.IsSuccess)
            {
                return Problem(
                    detail: result.ErrorMessage,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Cannot generate report");
            }

            logger.LogInformation("TrackEvent: {EventName}", "ParticipationReport_Requested");
            return Ok();
        }

        /// <summary>
        /// Sends participation report emails to all attendees with approved metrics.
        /// Must be called by an event lead.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Reports sent with count.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="403">User is not an event lead.</response>
        [HttpPost("send-all")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SendAllReports(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 SendAllParticipationReports Event={EventId}, User={UserId}", eventId, UserId);

            var result = await reportService.SendAllReportsAsync(eventId, UserId, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.ErrorMessage.Contains("event lead", StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                return Problem(
                    detail: result.ErrorMessage,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Cannot send reports");
            }

            logger.LogInformation("TrackEvent: {EventName}", "ParticipationReport_SentAll");
            return Ok(new { sentCount = result.Data });
        }
    }
}
