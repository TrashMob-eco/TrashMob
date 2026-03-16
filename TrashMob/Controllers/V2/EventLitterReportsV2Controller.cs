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
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for managing event litter reports.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/eventlitterreports")]
    public class EventLitterReportsV2Controller(
        IEventLitterReportManager eventLitterReportManager,
        IUserManager userManager,
        ILogger<EventLitterReportsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all event litter reports for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the event litter reports.</response>
        [HttpGet("by-event/{eventId}")]
        [ProducesResponseType(typeof(IReadOnlyList<FullEventLitterReport>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByEvent(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventLitterReports for Event={EventId}", eventId);

            var result = await eventLitterReportManager.GetByParentIdAsync(eventId, cancellationToken);
            var fullReports = await ToFullEventLitterReports(result, cancellationToken);

            return Ok(fullReports);
        }

        /// <summary>
        /// Gets the most recent event litter report for a litter report.
        /// </summary>
        /// <param name="litterReportId">The litter report ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the event litter report.</response>
        [HttpGet("by-litter-report/{litterReportId}")]
        [ProducesResponseType(typeof(FullEventLitterReport), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByLitterReport(Guid litterReportId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventLitterReportByLitterReportId LitterReport={LitterReportId}", litterReportId);

            var result = await eventLitterReportManager.GetAsync(l => l.LitterReportId == litterReportId, cancellationToken);
            var lastReport = result.OrderByDescending(e => e.CreatedDate).FirstOrDefault();
            var fullReport = await ToFullEventLitterReport(lastReport, cancellationToken);

            return Ok(fullReport);
        }

        /// <summary>
        /// Adds a new event litter report.
        /// </summary>
        /// <param name="eventLitterReport">The event litter report to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Event litter report created.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Add(EventLitterReport eventLitterReport, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddEventLitterReport for Event={EventId}", eventLitterReport.EventId);

            await eventLitterReportManager.AddAsync(eventLitterReport, UserId, cancellationToken);

            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Deletes an event litter report.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="litterReportId">The litter report ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Event litter report deleted.</response>
        [HttpDelete("{eventId}/{litterReportId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid eventId, Guid litterReportId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteEventLitterReport Event={EventId}, LitterReport={LitterReportId}", eventId, litterReportId);

            await eventLitterReportManager.Delete(eventId, litterReportId, cancellationToken);

            return NoContent();
        }

        private async Task<List<FullEventLitterReport>> ToFullEventLitterReports(
            IEnumerable<EventLitterReport> eventLitterReports, CancellationToken cancellationToken)
        {
            List<FullEventLitterReport> fullReports = [];

            foreach (var eventLitterReport in eventLitterReports)
            {
                var fullReport = await ToFullEventLitterReport(eventLitterReport, cancellationToken);
                fullReports.Add(fullReport);
            }

            return fullReports;
        }

        private async Task<FullEventLitterReport> ToFullEventLitterReport(
            EventLitterReport eventLitterReport, CancellationToken cancellationToken)
        {
            var user = await userManager.GetAsync(eventLitterReport.LitterReport.CreatedByUserId, cancellationToken);
            var fullLitterReport = eventLitterReport.LitterReport.ToFullLitterReport(user.UserName);

            return new FullEventLitterReport
            {
                EventId = eventLitterReport.EventId,
                LitterReportId = eventLitterReport.LitterReportId,
                LitterReport = fullLitterReport,
            };
        }
    }
}
