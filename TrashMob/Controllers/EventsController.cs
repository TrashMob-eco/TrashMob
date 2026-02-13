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
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    [Route("api/events")]
    public class EventsController(
        IKeyedManager<User> userManager,
        IEventManager eventManager,
        IEventAttendeeManager eventAttendeeManager)
        : SecureController
    {
        /// <summary>
        /// Gets a list of all events.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEvents(CancellationToken cancellationToken)
        {
            var result = await eventManager.GetAsync(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Gets a list of all active events.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Route("active")]
        [ProducesResponseType(typeof(List<DisplayEvent>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveEvents(CancellationToken cancellationToken)
        {
            var results = await eventManager.GetActiveEventsAsync(cancellationToken).ConfigureAwait(false);

            var displayResults = new List<DisplayEvent>();

            foreach (var mobEvent in results)
            {
                var user = await userManager.GetAsync(mobEvent.CreatedByUserId, cancellationToken);
                displayResults.Add(mobEvent.ToDisplayEvent(user.UserName));
            }

            return Ok(displayResults);
        }

        /// <summary>
        /// Gets a list of all completed events.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Route("completed")]
        [ProducesResponseType(typeof(List<DisplayEvent>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCompletedEvents(CancellationToken cancellationToken)
        {
            var results = await eventManager.GetCompletedEventsAsync(cancellationToken).ConfigureAwait(false);

            var displayResults = new List<DisplayEvent>();

            foreach (var mobEvent in results)
            {
                if (mobEvent.EventStatusId != (int)EventStatusEnum.Canceled)
                {
                    var user = await userManager.GetAsync(mobEvent.CreatedByUserId, cancellationToken);
                    displayResults.Add(mobEvent.ToDisplayEvent(user.UserName));
                }
            }

            return Ok(displayResults);
        }

        /// <summary>
        /// Retrieves all events that are not canceled.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Route("notcanceled")]
        [ProducesResponseType(typeof(List<DisplayEvent>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotCanceledEvents(CancellationToken cancellationToken)
        {
            var results = await eventManager.GetAsync(cancellationToken).ConfigureAwait(false);

            var displayResults = new List<DisplayEvent>();

            foreach (var mobEvent in results)
            {
                if (mobEvent.EventStatusId != (int)EventStatusEnum.Canceled)
                {
                    var user = await userManager.GetAsync(mobEvent.CreatedByUserId, cancellationToken);
                    displayResults.Add(mobEvent.ToDisplayEvent(user.UserName));
                }
            }

            return Ok(displayResults);
        }

        /// <summary>
        /// Gets a list of events that a specific user is attending.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Route("eventsuserisattending/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken)
        {
            var result = await eventAttendeeManager
                .GetEventsUserIsAttendingAsync(userId, cancellationToken: cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Gets a list of events for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="futureEventsOnly">When true, only return events in the future</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("userevents/{userId}/{futureEventsOnly}")]
        [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserEvents(Guid userId, bool futureEventsOnly,
            CancellationToken cancellationToken)
        {
            var result1 = await eventManager.GetUserEventsAsync(userId, futureEventsOnly, cancellationToken)
                .ConfigureAwait(false);
            var result2 = await eventAttendeeManager
                .GetEventsUserIsAttendingAsync(userId, futureEventsOnly, cancellationToken).ConfigureAwait(false);

            var allResults = result1.Union(result2, new EventComparer());
            return Ok(allResults);
        }

        /// <summary>
        /// Gets a list of canceled events for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="futureEventsOnly">When true, only return events in the future</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("canceleduserevents/{userId}/{futureEventsOnly}")]
        [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCanceledUserEvents(Guid userId, bool futureEventsOnly,
            CancellationToken cancellationToken)
        {
            var result1 = await eventManager.GetCanceledUserEventsAsync(userId, futureEventsOnly, cancellationToken)
                .ConfigureAwait(false);
            var result2 = await eventAttendeeManager
                .GetCanceledEventsUserIsAttendingAsync(userId, futureEventsOnly, cancellationToken)
                .ConfigureAwait(false);

            var allResults = result1.Union(result2, new EventComparer());
            return Ok(allResults);
        }

        /// <summary>
        /// Gets a specific event by its ID.
        /// </summary>
        /// <param name="id">The ID of the event.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEvent(Guid id, CancellationToken cancellationToken = default)
        {
            var mobEvent = await eventManager.GetAsync(id, cancellationToken).ConfigureAwait(false);

            if (mobEvent == null)
            {
                return NotFound();
            }

            return Ok(mobEvent);
        }

        /// <summary>
        /// Gets a list of filtered events based on the specified event filter.
        /// </summary>
        /// <param name="filter">The event filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Route("filteredevents")]
        [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilteredEvents([FromBody] EventFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await eventManager.GetFilteredEventsAsync(filter, cancellationToken).ConfigureAwait(false);

            if (filter.PageSize != null)
            {
                var pagedResults = result.OrderByDescending(e => e.EventDate).Skip(filter.PageIndex.GetValueOrDefault(0) * filter.PageSize.GetValueOrDefault(10))
                    .Take(filter.PageSize.GetValueOrDefault(10)).ToList();
                return Ok(pagedResults);
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets a paginated list of events for a given user, based on the specified event filter.
        /// </summary>
        /// <param name="filter">The event filter.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Route("pageduserevents/{userId}")]
        [ProducesResponseType(typeof(PaginatedList<Event>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedUserEvents([FromBody] EventFilter filter, Guid userId, CancellationToken cancellationToken)
        {
            var result1 = await eventManager.GetUserEventsAsync(filter, userId, cancellationToken)
                .ConfigureAwait(false);

            var result2 = await eventAttendeeManager
                .GetEventsUserIsAttendingAsync(filter, userId, cancellationToken).ConfigureAwait(false);

            var allResults = result1.Union(result2, new EventComparer());

            if (filter.PageSize != null)
            {
                var pagedResults = PaginatedList<Event>.Create(allResults.OrderByDescending(e => e.EventDate).AsQueryable(),
                                       filter.PageIndex.GetValueOrDefault(0), filter.PageSize.GetValueOrDefault(10));
                return Ok(pagedResults);
            }

            return Ok(allResults);
        }

        /// <summary>
        /// Gets a paginated list of events based on the specified event filter.
        /// </summary>
        /// <param name="filter">The event filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Route("pagedfilteredevents")]
        [ProducesResponseType(typeof(PaginatedList<Event>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedFilteredEvents([FromBody] EventFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await eventManager.GetFilteredEventsAsync(filter, cancellationToken).ConfigureAwait(false);

            if (filter.PageSize != null)
            {
                var pagedResults = PaginatedList<Event>.Create(result.OrderByDescending(e => e.EventDate).AsQueryable(),
                                       filter.PageIndex.GetValueOrDefault(0), filter.PageSize.GetValueOrDefault(10));
                return Ok(pagedResults);
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets a list of event locations within a specific time range.
        /// </summary>
        /// <param name="startTime">
        /// The start time of the range (ISO 8601 format).
        /// Example: <c>2024-06-10T15:30:00-07:00</c>
        /// </param>
        /// <param name="endTime">
        /// The end time of the range (ISO 8601 format).
        /// Example: <c>2025-06-10T15:30:00-07:00</c>
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Route("locationsbytimerange")]
        [ProducesResponseType(typeof(IEnumerable<Location>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventLocationsByTimeRange([FromQuery] DateTimeOffset? startTime,
            [FromQuery] DateTimeOffset? endTime, CancellationToken cancellationToken)
        {
            var result = await eventManager.GetEventLocationsByTimeRangeAsync(startTime, endTime, cancellationToken)
                .ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing event. Requires write scope.
        /// </summary>
        /// <param name="mobEvent">The event to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the updated event.</remarks>
        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEvent(Event mobEvent, CancellationToken cancellationToken)
        {
            var authResult =
                await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserIsEventLead);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            try
            {
                var updatedEvent = await eventManager.UpdateAsync(mobEvent, UserId, cancellationToken);
                TrackEvent(nameof(UpdateEvent));

                return Ok(updatedEvent);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventExists(mobEvent.Id, cancellationToken).ConfigureAwait(false))
                {
                    return NotFound();
                }

                throw;
            }
        }

        /// <summary>
        /// Adds a new event. Requires write scope.
        /// </summary>
        /// <param name="mobEvent">The event to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the added event.</remarks>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddEvent(Event mobEvent, CancellationToken cancellationToken)
        {
            var newEvent = await eventManager.AddAsync(mobEvent, UserId, cancellationToken).ConfigureAwait(false);
            TrackEvent(nameof(AddEvent));

            return Ok(newEvent);
        }

        /// <summary>
        /// Deletes an event. Requires write scope.
        /// </summary>
        /// <param name="eventCancellationRequest">The event cancellation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the ID of the deleted event.</remarks>
        [HttpDelete]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteEvent(EventCancellationRequest eventCancellationRequest,
            CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventCancellationRequest.EventId, cancellationToken)
                .ConfigureAwait(false);

            var authResult = await AuthorizationService.AuthorizeAsync(User, mobEvent,
                AuthorizationPolicyConstants.UserIsEventLeadOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await eventManager.DeleteAsync(eventCancellationRequest.EventId,
                eventCancellationRequest.CancellationReason, UserId, cancellationToken).ConfigureAwait(false);
            TrackEvent(nameof(DeleteEvent));

            return Ok(eventCancellationRequest.EventId);
        }

        private async Task<bool> EventExists(Guid id, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(id, cancellationToken).ConfigureAwait(false);

            return mobEvent != null;
        }
    }
}