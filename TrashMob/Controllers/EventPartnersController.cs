
namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventpartners")]
    public class EventPartnersController : SecureController
    {
        private readonly IEventPartnerManager eventPartnerManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IKeyedManager<PartnerLocation> partnerLocationManager;

        public EventPartnersController(IKeyedManager<Partner> partnerManager,
                                       IEventPartnerManager eventPartnerManager,
                                       IKeyedManager<PartnerLocation> partnerLocationManager) 
            : base()
        {
            this.eventPartnerManager = eventPartnerManager;
            this.partnerManager = partnerManager;
            this.partnerLocationManager = partnerLocationManager;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventPartners(Guid eventId, CancellationToken cancellationToken)
        {
            var displayEventPartners = await eventPartnerManager.GetByParentIdAsync(eventId, cancellationToken);
            return Ok(displayEventPartners);
        }

        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventPartner(EventPartner eventPartner, CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(eventPartner.PartnerId, cancellationToken);

            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var updatedEventPartner = await eventPartnerManager.UpdateAsync(eventPartner, UserId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(UpdateEventPartner));

            return Ok(updatedEventPartner);
        }

        [HttpPost]
        [Authorize(Policy = "ValidUser")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventPartner(EventPartner eventPartner, CancellationToken cancellationToken)
        {
            await eventPartnerManager.AddAsync(eventPartner, cancellationToken).ConfigureAwait(false);

            var partnerLocation = partnerLocationManager.GetAsync(eventPartner.PartnerLocationId, cancellationToken);

            TelemetryClient.TrackEvent(nameof(AddEventPartner));

            return Ok(partnerLocation);
        }
    }
}
