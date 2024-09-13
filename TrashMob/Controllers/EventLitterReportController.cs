namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventlitterreports")]
    public class EventLitterReportsController : SecureController
    {
        private readonly IEventLitterReportManager eventLitterReportManager;

        public EventLitterReportsController(IEventLitterReportManager eventLitterReportManager)
        {
            this.eventLitterReportManager = eventLitterReportManager;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventLitterReports(Guid eventId)
        {
            var result =
                (await eventLitterReportManager.GetByParentIdAsync(eventId, CancellationToken.None).ConfigureAwait(false))
                .Select(e => e.LitterReport.ToFullLitterReport("Unknown"));
            TelemetryClient.TrackEvent(nameof(GetEventLitterReports));
            return Ok(result);
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
    }
}