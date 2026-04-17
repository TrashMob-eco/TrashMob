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
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for event attendees as a nested resource under events.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/events/{eventId}/attendees")]
    public class EventAttendeesV2Controller(
        IEventAttendeeManager eventAttendeeManager,
        IUserWaiverManager userWaiverManager,
        IDependentWaiverManager dependentWaiverManager,
        IKeyedManager<User> userManager,
        IEmailManager emailManager,
        ILogger<EventAttendeesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets the count of active attendees for an event.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The attendee count.</returns>
        /// <response code="200">Returns the attendee count.</response>
        [HttpGet("count")]
        [ProducesResponseType(typeof(EventAttendeeCountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAttendeeCount(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetAttendeeCount requested for Event={EventId}", eventId);

            var count = await eventAttendeeManager.GetActiveAttendeeCountAsync(eventId, cancellationToken);

            return Ok(new EventAttendeeCountDto
            {
                EventId = eventId,
                Count = count,
            });
        }

        /// <summary>
        /// Gets a paginated list of active attendees for an event. Requires authentication.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="filter">Query parameters for pagination.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of event attendees with user info.</returns>
        /// <response code="200">Returns the paginated attendee list.</response>
        /// <response code="401">Authentication required.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(PagedResponse<EventAttendeeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAttendees(
            Guid eventId,
            [FromQuery] QueryParameters filter,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetAttendees requested for Event={EventId}, Page={Page}, PageSize={PageSize}",
                eventId, filter.Page, filter.PageSize);

            var query = eventAttendeeManager.GetEventAttendeesQueryable(eventId);
            var result = await query.ToPagedAsync(filter, ea => ea.ToV2Dto(), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Adds a new event attendee. Requires all waivers to be signed.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="dto">The event attendee DTO containing the UserId.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Attendee registered.</response>
        /// <response code="400">Waivers required.</response>
        /// <response code="409">Event is full.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(WaiverRequiredResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddEventAttendee(
            Guid eventId,
            EventAttendeeDto dto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddEventAttendee Event={EventId}, User={UserId}", eventId, dto.UserId);

            var eventAttendee = new EventAttendee { EventId = eventId, UserId = dto.UserId };

            // Check if user is a minor — minors cannot sign their own waivers
            var user = await userManager.GetAsync(dto.UserId, cancellationToken);

            if (user is { IsMinor: true, DependentId: not null })
            {
                // Minor path: check DependentWaivers signed by parent
                var hasValidDependentWaivers = await dependentWaiverManager
                    .HasValidWaiversForEventAsync(user.DependentId.Value, eventId, cancellationToken);

                if (!hasValidDependentWaivers)
                {
                    // Register with waiver-pending status and notify parent
                    eventAttendee.WaiverPendingDate = DateTimeOffset.UtcNow;

                    try
                    {
                        await eventAttendeeManager.AddAsync(eventAttendee, UserId, cancellationToken);
                    }
                    catch (InvalidOperationException ex)
                    {
                        return Conflict(ex.Message);
                    }

                    // Notify parent to sign waivers
                    await NotifyParentWaiverRequiredAsync(user, eventId, cancellationToken);

                    var requiredWaivers = await dependentWaiverManager
                        .GetRequiredWaiversForDependentEventAsync(user.DependentId.Value, eventId, cancellationToken);

                    return Ok(new { status = "waiver_pending", requiredWaiverCount = requiredWaivers.Count() });
                }

                // Minor has all waivers signed by parent — register normally
                try
                {
                    await eventAttendeeManager.AddAsync(eventAttendee, UserId, cancellationToken);

                    // Always notify parent when their minor registers for an event
                    await NotifyParentChildRegisteredAsync(user, eventId, cancellationToken);

                    return Ok();
                }
                catch (InvalidOperationException ex)
                {
                    return Conflict(ex.Message);
                }
            }

            // Adult path: check UserWaivers
            var hasValidWaiver = await userWaiverManager
                .HasValidWaiverForEventAsync(eventAttendee.UserId, eventId, cancellationToken);

            if (!hasValidWaiver)
            {
                var requiredWaivers = await userWaiverManager
                    .GetRequiredWaiversForEventAsync(eventAttendee.UserId, eventId, cancellationToken);

                return BadRequest(new WaiverRequiredResponse
                {
                    Message = "You must sign all required waivers before registering for this event.",
                    RequiredWaiverCount = requiredWaivers.Count(),
                    RequiredWaiverIds = requiredWaivers.Select(w => w.Id).ToList()
                });
            }

            try
            {
                await eventAttendeeManager.AddAsync(eventAttendee, UserId, cancellationToken);
                return Ok();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("full"))
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Event Full");
            }
        }

        /// <summary>
        /// Removes an attendee from an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Attendee removed.</response>
        [HttpDelete("{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteEventAttendee(Guid eventId, Guid userId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteEventAttendee Event={EventId}, User={UserId}", eventId, userId);

            await eventAttendeeManager.Delete(eventId, userId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Gets all event leads for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the event leads.</response>
        [HttpGet("leads")]
        [ProducesResponseType(typeof(IEnumerable<DisplayUser>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventLeads(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventLeads Event={EventId}", eventId);

            var result =
                (await eventAttendeeManager.GetEventLeadsAsync(eventId, cancellationToken))
                .Select(ea => ea.User.ToDisplayUser());
            return Ok(result);
        }

        /// <summary>
        /// Promotes an attendee to event lead. Only existing event leads can promote.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID to promote.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated event attendee.</response>
        /// <response code="400">Operation not allowed.</response>
        /// <response code="403">User is not an event lead.</response>
        [HttpPut("{userId}/promote")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PromoteToLead(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 PromoteToLead Event={EventId}, User={UserId}", eventId, userId);

            var isCurrentUserLead = await eventAttendeeManager.IsEventLeadAsync(eventId, UserId, cancellationToken);
            if (!isCurrentUserLead)
            {
                return Forbid();
            }

            try
            {
                var result = await eventAttendeeManager.PromoteToLeadAsync(eventId, userId, UserId, cancellationToken);
                return Ok(result.ToV2Dto());
            }
            catch (InvalidOperationException ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest, title: "Operation not allowed");
            }
        }

        /// <summary>
        /// Demotes an event lead to regular attendee. Only existing event leads can demote.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID to demote.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated event attendee.</response>
        /// <response code="400">Operation not allowed.</response>
        /// <response code="403">User is not an event lead.</response>
        [HttpPut("{userId}/demote")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DemoteFromLead(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DemoteFromLead Event={EventId}, User={UserId}", eventId, userId);

            var isCurrentUserLead = await eventAttendeeManager.IsEventLeadAsync(eventId, UserId, cancellationToken);
            if (!isCurrentUserLead)
            {
                return Forbid();
            }

            try
            {
                var result = await eventAttendeeManager.DemoteFromLeadAsync(eventId, userId, UserId, cancellationToken);
                return Ok(result.ToV2Dto());
            }
            catch (InvalidOperationException ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest, title: "Operation not allowed");
            }
        }

        /// <summary>
        /// Verifies an attendee's waiver status. Only event leads or admins can check.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the waiver status.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpGet("{userId}/waiver-status")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(WaiverCheckResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> VerifyAttendeeWaiverStatus(
            Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 VerifyWaiverStatus Event={EventId}, User={UserId}", eventId, userId);

            var isAdmin = User.IsInRole("Admin");
            var isEventLead = await eventAttendeeManager
                .IsEventLeadAsync(eventId, UserId, cancellationToken);

            if (!isAdmin && !isEventLead)
            {
                return Forbid();
            }

            var hasValidWaiver = await userWaiverManager
                .HasValidWaiverForEventAsync(userId, eventId, cancellationToken);

            return Ok(new WaiverCheckResultDto { HasValidWaiver = hasValidWaiver });
        }

        private async Task NotifyParentWaiverRequiredAsync(User minorUser, Guid eventId, CancellationToken cancellationToken)
        {
            if (minorUser.ParentUserId == null) return;

            var parent = await userManager.GetAsync(minorUser.ParentUserId.Value, cancellationToken);
            if (parent == null || string.IsNullOrWhiteSpace(parent.Email)) return;

            var evt = await eventAttendeeManager.GetEventsUserIsAttendingAsync(minorUser.Id, cancellationToken: cancellationToken);
            var eventInfo = evt.FirstOrDefault(e => e.Id == eventId);

            var childName = $"{minorUser.GivenName ?? minorUser.UserName}";
            var eventName = eventInfo?.Name ?? "an event";
            var eventDate = eventInfo?.EventDate.ToLocalTime().ToString("D") ?? "TBD";

            var subject = $"{childName} needs your waiver signature for {eventName}";
            var message = $"<p>{childName} has registered for <strong>{eventName}</strong> on {eventDate}, " +
                          $"but you need to sign the required waivers on their behalf before they can fully participate.</p>" +
                          $"<p>Please visit your <a href=\"https://www.trashmob.eco/mydashboard\">TrashMob dashboard</a> " +
                          $"to review and sign the waivers for {childName}.</p>";

            List<EmailAddress> recipients =
            [
                new() { Name = parent.DisplayFirstName, Email = parent.Email },
            ];

            var dynamicTemplateData = new
            {
                username = parent.DisplayFirstName,
                emailCopy = message,
                subject,
            };

            await emailManager.SendTemplatedEmailAsync(
                subject,
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General,
                dynamicTemplateData,
                recipients,
                cancellationToken);

            logger.LogInformation(
                "Parent waiver notification sent: Parent={ParentId}, Minor={MinorId}, Event={EventId}",
                parent.Id, minorUser.Id, eventId);
        }

        private async Task NotifyParentChildRegisteredAsync(User minorUser, Guid eventId, CancellationToken cancellationToken)
        {
            if (minorUser.ParentUserId == null) return;

            var parent = await userManager.GetAsync(minorUser.ParentUserId.Value, cancellationToken);
            if (parent == null || string.IsNullOrWhiteSpace(parent.Email)) return;

            var events = await eventAttendeeManager.GetEventsUserIsAttendingAsync(minorUser.Id, cancellationToken: cancellationToken);
            var eventInfo = events.FirstOrDefault(e => e.Id == eventId);

            var childName = $"{minorUser.GivenName ?? minorUser.UserName}";
            var eventName = eventInfo?.Name ?? "an event";
            var eventDate = eventInfo?.EventDate.ToLocalTime().ToString("D") ?? "TBD";

            var subject = $"{childName} has registered for {eventName}";
            var message = $"<p>{childName} has registered for <strong>{eventName}</strong> on {eventDate}.</p>" +
                          $"<p>All required waivers are already signed. No action is needed from you.</p>" +
                          $"<p>You can view your child's registrations on your <a href=\"https://www.trashmob.eco/mydashboard\">TrashMob dashboard</a>.</p>";

            List<EmailAddress> recipients =
            [
                new() { Name = parent.DisplayFirstName, Email = parent.Email },
            ];

            var dynamicTemplateData = new
            {
                username = parent.DisplayFirstName,
                emailCopy = message,
                subject,
            };

            await emailManager.SendTemplatedEmailAsync(
                subject,
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General,
                dynamicTemplateData,
                recipients,
                cancellationToken);

            logger.LogInformation(
                "Parent registration notification sent: Parent={ParentId}, Minor={MinorId}, Event={EventId}",
                parent.Id, minorUser.Id, eventId);
        }
    }
}
