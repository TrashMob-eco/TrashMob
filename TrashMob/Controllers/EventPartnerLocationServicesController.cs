
namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventpartnerlocationservices")]
    public class EventPartnerLocationServicesController : SecureController
    {
        private readonly IEventPartnerLocationServiceManager eventPartnerLocationServiceManager;
        private readonly IKeyedManager<Event> eventManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IPartnerLocationManager partnerLocationManager;
        private readonly IPartnerLocationContactManager partnerLocationContactManager;

        public EventPartnerLocationServicesController(IKeyedManager<Event> eventManager,
                                                      IKeyedManager<Partner> partnerManager,
                                                      IEventPartnerLocationServiceManager eventPartnerLocationServiceManager,
                                                      IPartnerLocationManager partnerLocationManager,
                                                      IPartnerLocationContactManager partnerLocationContactManager) 
            : base()
        {
            this.eventPartnerLocationServiceManager = eventPartnerLocationServiceManager;
            this.eventManager = eventManager;
            this.partnerManager = partnerManager;
            this.partnerLocationManager = partnerLocationManager;
            this.partnerLocationContactManager = partnerLocationContactManager;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventPartnerLocationServicesByEvent(Guid eventId, CancellationToken cancellationToken)
        {
            var displayEventPartners = await eventPartnerLocationServiceManager.GetByEventAsync(eventId, cancellationToken);
            TelemetryClient.TrackEvent(nameof(GetEventPartnerLocationServicesByEvent));
            return Ok(displayEventPartners);
        }

        [HttpGet("gethaulingpartnerlocation/{eventId}")]
        public async Task<IActionResult> GetHaulingPartnerLocation(Guid eventId, CancellationToken cancellationToken)
        {
            var partnerLocation = await eventPartnerLocationServiceManager.GetHaulingPartnerLocationForEvent(eventId, cancellationToken);
            TelemetryClient.TrackEvent(nameof(GetHaulingPartnerLocation));
            return Ok(partnerLocation);
        }

        [HttpGet("{eventId}/{partnerLocationId}")]
        public async Task<IActionResult> GetEventPartnerLocationServices(Guid eventId, Guid partnerLocationId, CancellationToken cancellationToken)
        {
            var displayEventPartners = await eventPartnerLocationServiceManager.GetByEventAndPartnerLocationAsync(eventId, partnerLocationId, cancellationToken);
            TelemetryClient.TrackEvent(nameof(GetEventPartnerLocationServices));
            return Ok(displayEventPartners);
        }

        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventPartnerLocationService(EventPartnerLocationService eventPartnerLocationService, CancellationToken cancellationToken = default)
        {
            var mobEvent = await eventManager.GetAsync(eventPartnerLocationService.EventId, cancellationToken);

            if (mobEvent == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var updatedEventPartnerLocationService = await eventPartnerLocationServiceManager.UpdateAsync(eventPartnerLocationService, UserId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(UpdateEventPartnerLocationService));

            return Ok(updatedEventPartnerLocationService);
        }

        [HttpPut("accept/{eventId}/{partnerLocationId}/{serviceId}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> ApproveEventPartnerLocationService(Guid eventId, Guid partnerLocationId, int serviceId, CancellationToken cancellationToken = default)
        {
            var partner = await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationId, cancellationToken);

            if (partner == null)
            {
                return NotFound();
            }

            var eventPartnerLocationServices = await eventPartnerLocationServiceManager.GetCurrentPartnersAsync(eventId, cancellationToken);

            if (eventPartnerLocationServices == null || !eventPartnerLocationServices.Any(epls => epls.ServiceTypeId == serviceId && epls.PartnerLocationId == partnerLocationId))
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var eventPartnerLocationService = eventPartnerLocationServices.FirstOrDefault(epls => epls.ServiceTypeId == serviceId);
            eventPartnerLocationService.EventPartnerLocationServiceStatusId = (int)EventPartnerLocationServiceStatusEnum.Accepted;

            var updatedEventPartnerLocationService = await eventPartnerLocationServiceManager.UpdateAsync(eventPartnerLocationService, UserId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(ApproveEventPartnerLocationService));

            return Ok(updatedEventPartnerLocationService);
        }

        [HttpPut("decline/{eventId}/{partnerLocationId}/{serviceId}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeclineEventPartnerLocationService(Guid eventId, Guid partnerLocationId, int serviceId, CancellationToken cancellationToken = default)
        {
            var partner = await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationId, cancellationToken);

            if (partner == null)
            {
                return NotFound();
            }

            var eventPartnerLocationServices = await eventPartnerLocationServiceManager.GetCurrentPartnersAsync(eventId, cancellationToken);

            if (eventPartnerLocationServices == null || !eventPartnerLocationServices.Any(epls => epls.ServiceTypeId == serviceId && epls.PartnerLocationId == partnerLocationId))
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var eventPartnerLocationService = eventPartnerLocationServices.FirstOrDefault(epls => epls.ServiceTypeId == serviceId);
            eventPartnerLocationService.EventPartnerLocationServiceStatusId = (int)EventPartnerLocationServiceStatusEnum.Declined;

            var updatedEventPartnerLocationService = await eventPartnerLocationServiceManager.UpdateAsync(eventPartnerLocationService, UserId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(ApproveEventPartnerLocationService));

            return Ok(updatedEventPartnerLocationService);
        }

        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventPartnerLocationService(EventPartnerLocationService eventPartnerLocationService, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventPartnerLocationService.EventId, cancellationToken);

            if (mobEvent == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await eventPartnerLocationServiceManager.AddAsync(eventPartnerLocationService, UserId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(AddEventPartnerLocationService));

            return Ok(result);
        }

        [HttpDelete("{eventId}/{partnerLocationId}/{serviceTypeId}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEventPartnerLocationService(Guid eventId, Guid partnerLocationId, int serviceTypeId, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);

            if (mobEvent == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await eventPartnerLocationServiceManager.DeleteAsync(eventId, partnerLocationId, serviceTypeId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(DeleteEventPartnerLocationService));

            return Ok(result);
        }
    }
}
