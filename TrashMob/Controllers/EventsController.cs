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
        [HttpGet]
        public async Task<IActionResult> GetEvents(CancellationToken cancellationToken)
        {
            var result = await eventManager.GetAsync(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Route("active")]
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

        [HttpGet]
        [Route("completed")]
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

        [HttpGet]
        [Route("notcanceled")]
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

        [HttpGet]
        [Route("eventsuserisattending/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        public async Task<IActionResult> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken)
        {
            var result = await eventAttendeeManager
                .GetEventsUserIsAttendingAsync(userId, cancellationToken: cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("userevents/{userId}/{futureEventsOnly}")]
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

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("canceleduserevents/{userId}/{futureEventsOnly}")]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(Guid id, CancellationToken cancellationToken = default)
        {
            var mobEvent = await eventManager.GetAsync(id, cancellationToken).ConfigureAwait(false);

            if (mobEvent == null)
            {
                return NotFound();
            }

            return Ok(mobEvent);
        }

        [HttpGet]
        [Route("filteredevents")]
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

        [HttpGet]
        [Route("pagedfilteredevents")]
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

        [HttpGet]
        [Route("locationsbytimerange")]
        public async Task<IActionResult> GetEventLocationsByTimeRange([FromQuery] DateTimeOffset? startTime,
            [FromQuery] DateTimeOffset? endTime, CancellationToken cancellationToken)
        {
            var result = await eventManager.GeEventLocationsByTimeRangeAsync(startTime, endTime, cancellationToken)
                .ConfigureAwait(false);

            return Ok(result);
        }

        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEvent(Event mobEvent, CancellationToken cancellationToken)
        {
            var authResult =
                await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            try
            {
                var updatedEvent = await eventManager.UpdateAsync(mobEvent, UserId, cancellationToken);
                TelemetryClient.TrackEvent(nameof(UpdateEvent));

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

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEvent(Event mobEvent, CancellationToken cancellationToken)
        {
            var newEvent = await eventManager.AddAsync(mobEvent, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddEvent));

            return Ok(newEvent);
        }

        [HttpDelete]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEvent(EventCancellationRequest eventCancellationRequest,
            CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventCancellationRequest.EventId, cancellationToken)
                .ConfigureAwait(false);

            var authResult = await AuthorizationService.AuthorizeAsync(User, mobEvent,
                AuthorizationPolicyConstants.UserOwnsEntityOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await eventManager.DeleteAsync(eventCancellationRequest.EventId,
                eventCancellationRequest.CancellationReason, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteEvent));

            return Ok(eventCancellationRequest.EventId);
        }

        private async Task<bool> EventExists(Guid id, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(id, cancellationToken).ConfigureAwait(false);

            return mobEvent != null;
        }
    }
}