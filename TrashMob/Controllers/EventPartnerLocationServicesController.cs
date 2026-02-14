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
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventpartnerlocationservices")]
    public class EventPartnerLocationServicesController(
        IKeyedManager<Event> eventManager,
        IEventPartnerLocationServiceManager eventPartnerLocationServiceManager,
        IPartnerLocationManager partnerLocationManager)
        : SecureController
    {

        /// <summary>
        /// Gets a list of all event partner location services for a given event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{eventId}")]
        [ProducesResponseType(typeof(IEnumerable<DisplayEventPartnerLocation>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventPartnerLocationServicesByEvent(Guid eventId,
            CancellationToken cancellationToken)
        {
            var displayEventPartners =
                await eventPartnerLocationServiceManager.GetByEventAsync(eventId, cancellationToken);
            TrackEvent(nameof(GetEventPartnerLocationServicesByEvent));
            return Ok(displayEventPartners);
        }

        /// <summary>
        /// Gets the hauling partner location for a given event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("gethaulingpartnerlocation/{eventId}")]
        [ProducesResponseType(typeof(PartnerLocation), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHaulingPartnerLocation(Guid eventId, CancellationToken cancellationToken)
        {
            var partnerLocation =
                await eventPartnerLocationServiceManager.GetHaulingPartnerLocationForEvent(eventId, cancellationToken);
            TrackEvent(nameof(GetHaulingPartnerLocation));
            return Ok(partnerLocation);
        }

        /// <summary>
        /// Gets a list of event partner location services for a specific event and partner location.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{eventId}/{partnerLocationId}")]
        [ProducesResponseType(typeof(IEnumerable<DisplayEventPartnerLocationService>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventPartnerLocationServices(Guid eventId, Guid partnerLocationId,
            CancellationToken cancellationToken)
        {
            var displayEventPartners =
                await eventPartnerLocationServiceManager.GetByEventAndPartnerLocationAsync(eventId, partnerLocationId,
                    cancellationToken);
            TrackEvent(nameof(GetEventPartnerLocationServices));
            return Ok(displayEventPartners);
        }

        /// <summary>
        /// Updates an event partner location service. Requires write scope.
        /// </summary>
        /// <param name="eventPartnerLocationService">The event partner location service to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The updated event partner location service.</remarks>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventPartnerLocationService), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEventPartnerLocationService(
            EventPartnerLocationService eventPartnerLocationService, CancellationToken cancellationToken = default)
        {
            var mobEvent = await eventManager.GetAsync(eventPartnerLocationService.EventId, cancellationToken);

            if (mobEvent == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var updatedEventPartnerLocationService = await eventPartnerLocationServiceManager
                .UpdateAsync(eventPartnerLocationService, UserId, cancellationToken);

            TrackEvent(nameof(UpdateEventPartnerLocationService));

            return Ok(updatedEventPartnerLocationService);
        }

        /// <summary>
        /// Approves an event partner location service. Requires write scope.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="serviceId">The service ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The approved event partner location service.</remarks>
        [HttpPut("accept/{eventId}/{partnerLocationId}/{serviceId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventPartnerLocationService), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveEventPartnerLocationService(Guid eventId, Guid partnerLocationId,
            int serviceId, CancellationToken cancellationToken = default)
        {
            var partner = await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationId, cancellationToken);

            if (partner == null)
            {
                return NotFound();
            }

            var eventPartnerLocationServices =
                await eventPartnerLocationServiceManager.GetCurrentPartnersAsync(eventId, cancellationToken);

            if (eventPartnerLocationServices == null || !eventPartnerLocationServices.Any(epls =>
                    epls.ServiceTypeId == serviceId && epls.PartnerLocationId == partnerLocationId))
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var eventPartnerLocationService =
                eventPartnerLocationServices.FirstOrDefault(epls => epls.ServiceTypeId == serviceId);
            eventPartnerLocationService.EventPartnerLocationServiceStatusId =
                (int)EventPartnerLocationServiceStatusEnum.Accepted;

            var updatedEventPartnerLocationService = await eventPartnerLocationServiceManager
                .UpdateAsync(eventPartnerLocationService, UserId, cancellationToken);

            TrackEvent(nameof(ApproveEventPartnerLocationService));

            return Ok(updatedEventPartnerLocationService);
        }

        /// <summary>
        /// Declines an event partner location service. Requires write scope.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="serviceId">The service ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The declined event partner location service.</remarks>
        [HttpPut("decline/{eventId}/{partnerLocationId}/{serviceId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventPartnerLocationService), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeclineEventPartnerLocationService(Guid eventId, Guid partnerLocationId,
            int serviceId, CancellationToken cancellationToken = default)
        {
            var partner = await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationId, cancellationToken);

            if (partner == null)
            {
                return NotFound();
            }

            var eventPartnerLocationServices =
                await eventPartnerLocationServiceManager.GetCurrentPartnersAsync(eventId, cancellationToken);

            if (eventPartnerLocationServices == null || !eventPartnerLocationServices.Any(epls =>
                    epls.ServiceTypeId == serviceId && epls.PartnerLocationId == partnerLocationId))
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var eventPartnerLocationService =
                eventPartnerLocationServices.FirstOrDefault(epls => epls.ServiceTypeId == serviceId);
            eventPartnerLocationService.EventPartnerLocationServiceStatusId =
                (int)EventPartnerLocationServiceStatusEnum.Declined;

            var updatedEventPartnerLocationService = await eventPartnerLocationServiceManager
                .UpdateAsync(eventPartnerLocationService, UserId, cancellationToken);

            TrackEvent(nameof(ApproveEventPartnerLocationService));

            return Ok(updatedEventPartnerLocationService);
        }

        /// <summary>
        /// Adds a new event partner location service. Requires write scope.
        /// </summary>
        /// <param name="eventPartnerLocationService">The event partner location service to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the newly created event partner location service.</remarks>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventPartnerLocationService), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddEventPartnerLocationService(
            EventPartnerLocationService eventPartnerLocationService, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventPartnerLocationService.EventId, cancellationToken);

            if (mobEvent == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var result = await eventPartnerLocationServiceManager
                .AddAsync(eventPartnerLocationService, UserId, cancellationToken);

            TrackEvent(nameof(AddEventPartnerLocationService));

            return Ok(result);
        }

        /// <summary>
        /// Deletes an event partner location service by event, partner location, and service type. Requires write scope.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="serviceTypeId">The service type ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the number of entities deleted.</remarks>
        [HttpDelete("{eventId}/{partnerLocationId}/{serviceTypeId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEventPartnerLocationService(Guid eventId, Guid partnerLocationId,
            int serviceTypeId, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);

            if (mobEvent == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            await eventPartnerLocationServiceManager
                .DeleteAsync(eventId, partnerLocationId, serviceTypeId, cancellationToken);

            TrackEvent(nameof(DeleteEventPartnerLocationService));

            return NoContent();
        }
    }
}