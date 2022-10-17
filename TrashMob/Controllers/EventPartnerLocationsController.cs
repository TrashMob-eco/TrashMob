
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
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventpartnerlocations")]
    public class EventPartnerLocationsController : SecureController
    {
        private readonly IEventPartnerLocationManager eventPartnerLocationManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IKeyedManager<PartnerLocation> partnerLocationManager;

        public EventPartnerLocationsController(IKeyedManager<Partner> partnerManager,
                                               IEventPartnerLocationManager eventPartnerLocationManager,
                                               IKeyedManager<PartnerLocation> partnerLocationManager) 
            : base()
        {
            this.eventPartnerLocationManager = eventPartnerLocationManager;
            this.partnerManager = partnerManager;
            this.partnerLocationManager = partnerLocationManager;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventPartners(Guid eventId, CancellationToken cancellationToken)
        {
            var displayEventPartners = await eventPartnerLocationManager.GetByParentIdAsync(eventId, cancellationToken);
            return Ok(displayEventPartners);
        }

        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventPartner(EventPartnerLocation eventPartnerLocation, CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(eventPartnerLocation.PartnerLocation.PartnerId, cancellationToken);

            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var updatedEventPartner = await eventPartnerLocationManager.UpdateAsync(eventPartnerLocation, UserId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(UpdateEventPartner));

            return Ok(updatedEventPartner);
        }

        [HttpPost]
        [Authorize(Policy = "ValidUser")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventPartner(EventPartnerLocation eventPartner, CancellationToken cancellationToken)
        {
            await eventPartnerLocationManager.AddAsync(eventPartner, cancellationToken).ConfigureAwait(false);

            var partnerLocation = partnerLocationManager.GetAsync(eventPartner.PartnerLocationId, cancellationToken);

            TelemetryClient.TrackEvent(nameof(AddEventPartner));

            return Ok(partnerLocation);
        }
    }
}
