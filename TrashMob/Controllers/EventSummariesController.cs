namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventsummaries")]
    public class EventSummariesController(
        IEventSummaryManager eventSummaryManager,
        IKeyedManager<Event> eventManager)
        : SecureController
    {

        /// <summary>
        /// Gets the event summary for a given event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The event summary.</remarks>
        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventSummary(Guid eventId, CancellationToken cancellationToken = default)
        {
            var eventSummary =
                (await eventSummaryManager.GetAsync(es => es.EventId == eventId, cancellationToken)).FirstOrDefault();

            if (eventSummary is not null)
            {
                return Ok(eventSummary);
            }

            return NotFound();
        }

        /// <summary>
        /// Gets the event summary (v2) for a given event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The event summary (v2).</remarks>
        [HttpGet("v2/{eventId}")]
        public async Task<IActionResult> GetEventSummaryV2(Guid eventId, CancellationToken cancellationToken = default)
        {
            var eventSummary =
                (await eventSummaryManager.GetAsync(es => es.EventId == eventId, cancellationToken)).FirstOrDefault();

            if (eventSummary is null)
            {
                eventSummary = new EventSummary
                {
                    EventId = eventId,
                };
            }

            return Ok(eventSummary);
        }

        /// <summary>
        /// Gets the event summaries based on the provided location filters.
        /// </summary>
        /// <param name="country">The country filter.</param>
        /// <param name="region">The region filter.</param>
        /// <param name="city">The city filter.</param>
        /// <param name="postalCode">The postal code filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>A list of event summaries matching the filters.</remarks>
        [HttpGet]
        public async Task<IActionResult> GetEventSummaries([FromQuery] string country = "",
            [FromQuery] string region = "", [FromQuery] string city = "", [FromQuery] string postalCode = "",
            CancellationToken cancellationToken = default)
        {
            var locationFilter = new LocationFilter
            {
                City = city,
                Region = region,
                Country = country,
                PostalCode = postalCode,
            };

            var eventSummaries = await eventSummaryManager.GetFilteredAsync(locationFilter, cancellationToken);

            return Ok(eventSummaries);
        }

        /// <summary>
        /// Updates an existing event summary.
        /// </summary>
        /// <param name="eventSummary">The event summary to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The updated event summary.</remarks>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventSummary(EventSummary eventSummary,
            CancellationToken cancellationToken)
        {
            if (!await IsAuthorizedAsync(eventSummary, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var updatedEvent = await eventSummaryManager.UpdateAsync(eventSummary, UserId, cancellationToken);
            TrackEvent(nameof(UpdateEventSummary));

            return Ok(updatedEvent);
        }

        /// <summary>
        /// Adds a new event summary.
        /// </summary>
        /// <param name="eventSummary">The event summary to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The created event summary.</remarks>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventSummary(EventSummary eventSummary, CancellationToken cancellationToken)
        {
            if (!await IsAuthorizedAsync(eventSummary, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var result = await eventSummaryManager.AddAsync(eventSummary, UserId, cancellationToken);

            TrackEvent(nameof(AddEventSummary));

            return CreatedAtAction(nameof(GetEventSummary), new { eventId = eventSummary.EventId }, result);
        }

        /// <summary>
        /// Deletes an event summary.
        /// </summary>
        /// <param name="eventId">The event ID of the summary to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The ID of the deleted event summary.</remarks>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEventSummary(Guid eventId, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            await eventSummaryManager.DeleteAsync(eventId, cancellationToken);
            TrackEvent(nameof(DeleteEventSummary));

            return NoContent();
        }
    }
}