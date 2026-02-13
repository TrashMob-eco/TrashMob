namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventlitterreports")]
    public class EventLitterReportsController(IEventLitterReportManager eventLitterReportManager, IUserManager userManager) : SecureController
    {
        private readonly IEventLitterReportManager eventLitterReportManager = eventLitterReportManager;
        private readonly IUserManager userManager = userManager;

        /// <summary>
        /// Gets a list of all event litter reports for a given event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{eventId}")]
        [ProducesResponseType(typeof(IEnumerable<EventLitterReport>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventLitterReports(Guid eventId, CancellationToken cancellationToken)
        {
            var result = await eventLitterReportManager.GetByParentIdAsync(eventId, cancellationToken)
                .ConfigureAwait(false);

            var fullEventLitterReports = await ToFullEventLitterReports(result, cancellationToken);

            TrackEvent(nameof(GetEventLitterReports));
            return Ok(fullEventLitterReports);
        }

        /// <summary>
        /// Gets an event litter report by litter report ID.
        /// </summary>
        /// <param name="litterReportId">The litter report ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("GetByLitterReportId/{litterReportId}")]
        [ProducesResponseType(typeof(FullEventLitterReport), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventLitterReportByLitterReportId(Guid litterReportId, CancellationToken cancellationToken)
        {
            var result = await eventLitterReportManager.GetAsync(l => l.LitterReportId == litterReportId, cancellationToken)
                .ConfigureAwait(false);

            var lastEventLitterReport = result.OrderByDescending(e => e.CreatedDate).FirstOrDefault();

            var fullEventLitterReport = await ToFullEventLitterReport(lastEventLitterReport, cancellationToken);

            TrackEvent(nameof(GetEventLitterReportByLitterReportId));
            return Ok(fullEventLitterReport);
        }

        /// <summary>
        /// Updates an existing event litter report.
        /// </summary>
        /// <param name="eventLitterReport">The event litter report to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The updated event litter report.</remarks>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventLitterReport), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEventLitterReport(EventLitterReport eventLitterReport,
            CancellationToken cancellationToken)
        {
            var authResult =
                await AuthorizationService.AuthorizeAsync(User, eventLitterReport,
                    AuthorizationPolicyConstants.UserIsEventLead);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            try
            {
                var updatedEventLitterReport = await eventLitterReportManager
                    .UpdateAsync(eventLitterReport, UserId, cancellationToken).ConfigureAwait(false);
                TrackEvent(nameof(UpdateEventLitterReport));

                return Ok(updatedEventLitterReport);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventLitterReportExists(eventLitterReport.EventId, eventLitterReport.LitterReportId, cancellationToken)
                        .ConfigureAwait(false))
                {
                    return NotFound();
                }

                throw;
            }
        }

        /// <summary>
        /// Adds a new event litter report.
        /// </summary>
        /// <param name="eventLitterReport">The event litter report to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddEventLitterReport(EventLitterReport eventLitterReport,
            CancellationToken cancellationToken)
        {
            await eventLitterReportManager.AddAsync(eventLitterReport, UserId, cancellationToken).ConfigureAwait(false);
            TrackEvent(nameof(AddEventLitterReport));
            return Ok();
        }

        /// <summary>
        /// Deletes an event litter report.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="litterReportId">The litter report ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{eventId}/{litterReportId}")]
        // Todo: Tighten this down
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(void), 204)]
        public async Task<IActionResult> DeleteEventLitterReport(Guid eventId, Guid litterReportId,
            CancellationToken cancellationToken)
        {
            await eventLitterReportManager.Delete(eventId, litterReportId, cancellationToken).ConfigureAwait(false);
            TrackEvent(nameof(DeleteEventLitterReport));

            return new NoContentResult();
        }

        private async Task<bool> EventLitterReportExists(Guid eventId, Guid litterReportId, CancellationToken cancellationToken)
        {
            var litterReport = await eventLitterReportManager
                .GetAsync(ea => ea.EventId == eventId && ea.LitterReportId == litterReportId, cancellationToken).ConfigureAwait(false);

            return litterReport?.FirstOrDefault() != null;
        }

        private async Task<IEnumerable<FullEventLitterReport>> ToFullEventLitterReports(IEnumerable<EventLitterReport> eventLitterReports, CancellationToken cancellationToken)
        {
            var fullEventLitterReports = new List<FullEventLitterReport>();

            foreach (var eventLitterReport in eventLitterReports)
            {
                var fullEventLitterReport = await ToFullEventLitterReport(eventLitterReport, cancellationToken);
                fullEventLitterReports.Add(fullEventLitterReport);
            }

            return fullEventLitterReports;
        }

        private async Task<FullEventLitterReport> ToFullEventLitterReport(EventLitterReport eventLitterReport, CancellationToken cancellationToken)
        {
            var fullLitterReport = await ToFullLitterReport(eventLitterReport.LitterReport, cancellationToken);

            var fullEventLitterReport = new FullEventLitterReport
            {
                EventId = eventLitterReport.EventId,
                LitterReportId = eventLitterReport.LitterReportId,
                LitterReport = fullLitterReport
            };

            return fullEventLitterReport;
        }

        private async Task<FullLitterReport> ToFullLitterReport(LitterReport litterReport, CancellationToken cancellationToken)
        {
            var user = await userManager.GetAsync(litterReport.CreatedByUserId, cancellationToken);
            return litterReport.ToFullLitterReport(user.UserName);
        }
    }
}