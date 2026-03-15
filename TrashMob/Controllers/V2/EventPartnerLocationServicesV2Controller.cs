namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using System.Linq;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for managing event partner location services.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/eventpartnerlocationservices")]
    public class EventPartnerLocationServicesV2Controller(
        IEventPartnerLocationServiceManager eventPartnerLocationServiceManager,
        IKeyedManager<Event> eventManager,
        IPartnerLocationManager partnerLocationManager,
        IAuthorizationService authorizationService,
        ILogger<EventPartnerLocationServicesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all partner locations for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner locations.</response>
        [HttpGet("{eventId}")]
        [ProducesResponseType(typeof(IReadOnlyList<DisplayEventPartnerLocation>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByEvent(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventPartnerLocationServices for Event={EventId}", eventId);

            var result = await eventPartnerLocationServiceManager.GetByEventAsync(eventId, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Gets the hauling partner location for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the hauling partner location.</response>
        [HttpGet("hauling/{eventId}")]
        [ProducesResponseType(typeof(PartnerLocation), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHaulingPartnerLocation(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetHaulingPartnerLocation for Event={EventId}", eventId);

            var result = await eventPartnerLocationServiceManager.GetHaulingPartnerLocationForEvent(eventId, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Gets partner location services for a specific event and partner location.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner location services.</response>
        [HttpGet("{eventId}/{partnerLocationId}")]
        [ProducesResponseType(typeof(IReadOnlyList<DisplayEventPartnerLocationService>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByEventAndPartnerLocation(
            Guid eventId, Guid partnerLocationId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventPartnerLocationServices Event={EventId}, PartnerLocation={PartnerLocationId}",
                eventId, partnerLocationId);

            var result = await eventPartnerLocationServiceManager.GetByEventAndPartnerLocationAsync(
                eventId, partnerLocationId, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Adds a new event partner location service.
        /// </summary>
        /// <param name="dto">The service to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Service created.</response>
        /// <response code="403">Not authorized.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Add(
            EventPartnerLocationServiceRequestDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddEventPartnerLocationService for Event={EventId}", dto.EventId);

            var mobEvent = await eventManager.GetAsync(dto.EventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            await eventPartnerLocationServiceManager.AddAsync(entity, UserId, cancellationToken);

            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Updates an event partner location service.
        /// </summary>
        /// <param name="dto">The service to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Service updated.</response>
        /// <response code="403">Not authorized.</response>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            EventPartnerLocationServiceRequestDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateEventPartnerLocationService for Event={EventId}", dto.EventId);

            var mobEvent = await eventManager.GetAsync(dto.EventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            await eventPartnerLocationServiceManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Accepts or declines an event partner location service.
        /// </summary>
        /// <param name="acceptDecline">Either "accept" or "decline".</param>
        /// <param name="eventId">The event ID.</param>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="serviceTypeId">The service type ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated service.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Service or partner not found.</response>
        [HttpPut("{acceptDecline}/{eventId}/{partnerLocationId}/{serviceTypeId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventPartnerLocationService), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AcceptOrDecline(
            string acceptDecline, Guid eventId, Guid partnerLocationId, int serviceTypeId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 {Action}EventPartnerLocationService Event={EventId}, PartnerLocation={PartnerLocationId}, ServiceType={ServiceTypeId}",
                acceptDecline, eventId, partnerLocationId, serviceTypeId);

            if (acceptDecline != "accept" && acceptDecline != "decline")
            {
                return Problem(detail: "Action must be 'accept' or 'decline'.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid action");
            }

            var partner = await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            var eventPartnerLocationServices =
                await eventPartnerLocationServiceManager.GetCurrentPartnersAsync(eventId, cancellationToken);

            if (eventPartnerLocationServices is null || !eventPartnerLocationServices.Any(epls =>
                    epls.ServiceTypeId == serviceTypeId && epls.PartnerLocationId == partnerLocationId))
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var eventPartnerLocationService =
                eventPartnerLocationServices.FirstOrDefault(epls => epls.ServiceTypeId == serviceTypeId);

            eventPartnerLocationService.EventPartnerLocationServiceStatusId = acceptDecline == "accept"
                ? (int)EventPartnerLocationServiceStatusEnum.Accepted
                : (int)EventPartnerLocationServiceStatusEnum.Declined;

            var updatedService = await eventPartnerLocationServiceManager
                .UpdateAsync(eventPartnerLocationService, UserId, cancellationToken);

            return Ok(updatedService);
        }

        /// <summary>
        /// Deletes an event partner location service.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="serviceTypeId">The service type ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Service deleted.</response>
        /// <response code="403">Not authorized.</response>
        [HttpDelete("{eventId}/{partnerLocationId}/{serviceTypeId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            Guid eventId, Guid partnerLocationId, int serviceTypeId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteEventPartnerLocationService Event={EventId}, PartnerLocation={PartnerLocationId}, ServiceType={ServiceTypeId}",
                eventId, partnerLocationId, serviceTypeId);

            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            await eventPartnerLocationServiceManager.DeleteAsync(eventId, partnerLocationId, serviceTypeId, cancellationToken);

            return NoContent();
        }

        private async Task<bool> IsAuthorizedAsync(object resource, string policy)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var authResult = await authorizationService.AuthorizeAsync(User, resource, policy);
            return authResult.Succeeded;
        }
    }
}
