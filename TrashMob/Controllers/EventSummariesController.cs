
namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    [Route("api/eventsummaries")]
    public class EventSummariesController : SecureController
    {
        private readonly IEventSummaryManager eventSummaryManager;
        private readonly IKeyedManager<Event> eventManager;

        public EventSummariesController(IEventSummaryManager eventSummaryManager,
                                        IKeyedManager<Event> eventManager) 
            : base()
        {
            this.eventSummaryManager = eventSummaryManager;
            this.eventManager = eventManager;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventSummary(Guid eventId, CancellationToken cancellationToken = default)
        {
            var eventSummary = await eventSummaryManager.GetAsync(es => es.EventId == eventId, cancellationToken).ConfigureAwait(false);

            if (eventSummary != null)
            {
                return Ok(eventSummary);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetEventSummaries([FromQuery] string country = "", [FromQuery] string region = "", [FromQuery] string city = "", [FromQuery] string postalCode = "", CancellationToken cancellationToken = default)
        {
            var locationFilter = new LocationFilter()
            {
                City = city,
                Region = region,
                Country = country,
                PostalCode = postalCode
            };

            var eventSummaries = await eventSummaryManager.GetFilteredAsync(locationFilter, cancellationToken);

            return Ok(eventSummaries);
        }

        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventSummary(EventSummary eventSummary, CancellationToken cancellationToken)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, eventSummary, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var updatedEvent = await eventSummaryManager.UpdateAsync(eventSummary, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdateEventSummary));

            return Ok(updatedEvent);
        }

        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventSummary(EventSummary eventSummary, CancellationToken cancellationToken)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, eventSummary, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await eventSummaryManager.AddAsync(eventSummary, UserId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(AddEventSummary));

            return CreatedAtAction(nameof(GetEventSummary), new { eventId = eventSummary.EventId }, result);
        }

        [HttpDelete("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEventSummary(Guid eventId, CancellationToken cancellationToken)
        {
            var mobEvent = eventManager.GetAsync(eventId, cancellationToken);

            var authResult = await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await eventSummaryManager.DeleteAsync(eventId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteEventSummary));

            return Ok(eventId);
        }
    }
}
