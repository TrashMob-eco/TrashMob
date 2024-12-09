namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventLitterReports(Guid eventId)
        {
            var result = await eventLitterReportManager.GetByParentIdAsync(eventId, CancellationToken.None)
                .ConfigureAwait(false);

            var fullEventLitterReports = await ToFullEventLitterReports(result, CancellationToken.None);

            TelemetryClient.TrackEvent(nameof(GetEventLitterReports));
            return Ok(fullEventLitterReports);
        }

        [HttpGet("GetByLitterReportId/{litterReportId}")]
        public async Task<IActionResult> GetEventLitterReportByLitterReportId(Guid litterReportId)
        {
            var result = await eventLitterReportManager.GetAsync(l => l.LitterReportId == litterReportId, CancellationToken.None)
                .ConfigureAwait(false);

            var lastEventLitterReport = result.OrderByDescending(e => e.CreatedDate).FirstOrDefault();

            var fullEventLitterReport = await ToFullEventLitterReport(lastEventLitterReport, CancellationToken.None);

            TelemetryClient.TrackEvent(nameof(GetEventLitterReportByLitterReportId));
            return Ok(fullEventLitterReport);
        }

        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventLitterReport(EventLitterReport eventLitterReport,
            CancellationToken cancellationToken)
        {
            var authResult =
                await AuthorizationService.AuthorizeAsync(User, eventLitterReport,
                    AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            try
            {
                var updatedEventLitterReport = await eventLitterReportManager
                    .UpdateAsync(eventLitterReport, UserId, cancellationToken).ConfigureAwait(false);
                TelemetryClient.TrackEvent(nameof(UpdateEventLitterReport));

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

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventLitterReport(EventLitterReport eventLitterReport,
            CancellationToken cancellationToken)
        {
            await eventLitterReportManager.AddAsync(eventLitterReport, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddEventLitterReport));
            return Ok();
        }

        [HttpDelete("{eventId}/{litterReportId}")]
        // Todo: Tighten this down
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEventLitterReport(Guid eventId, Guid litterReportId,
            CancellationToken cancellationToken)
        {
            await eventLitterReportManager.Delete(eventId, litterReportId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteEventLitterReport));

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