
namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/partnerevents")]
    public class PartnerEventsController : SecureController
    {
        private readonly IBaseManager<EventPartner> eventPartnerManager;
        private readonly IKeyedManager<PartnerLocation> partnerLocationManager;
        private readonly IKeyedManager<Event> eventManager;
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnerEventsController(IBaseManager<EventPartner> eventPartnerManager,
                                       IKeyedManager<PartnerLocation> partnerLocationManager,
                                       IKeyedManager<Event> eventManager,
                                       IKeyedManager<Partner> partnerManager)
            : base()
        {
            this.eventPartnerManager = eventPartnerManager;
            this.partnerLocationManager = partnerLocationManager;
            this.eventManager = eventManager;
            this.partnerManager = partnerManager;
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetPartnerEvents(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.Get(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var displayPartnerEvents = new List<DisplayPartnerEvent>();
            var currentPartners = await eventPartnerManager.Get(p => p.PartnerId == partnerId, cancellationToken).ConfigureAwait(false);

            if (currentPartners.Any())
            {
                // Convert the current list of partner events for the event to a display partner (reduces round trips)
                foreach (var cp in currentPartners.ToList())
                {
                    var displayPartnerEvent = new DisplayPartnerEvent
                    {
                        EventId = cp.EventId,
                        PartnerId = partnerId,
                        PartnerLocationId = cp.PartnerLocationId,
                        EventPartnerStatusId = cp.EventPartnerStatusId,
                    };

                    displayPartnerEvent.PartnerName = partner.Name;

                    var partnerLocation = (await partnerLocationManager.Get(pl => pl.PartnerId == cp.PartnerId && pl.Id == cp.PartnerLocationId, cancellationToken)).FirstOrDefault();

                    displayPartnerEvent.PartnerLocationName = partnerLocation.Name;

                    var mobEvent = await eventManager.Get(cp.EventId, cancellationToken).ConfigureAwait(false);

                    displayPartnerEvent.EventName = mobEvent.Name;
                    displayPartnerEvent.EventStreetAddress = mobEvent.StreetAddress;
                    displayPartnerEvent.EventCity = mobEvent.City;
                    displayPartnerEvent.EventRegion = mobEvent.Region;
                    displayPartnerEvent.EventCountry = mobEvent.Country;
                    displayPartnerEvent.EventPostalCode = mobEvent.PostalCode;
                    displayPartnerEvent.EventDescription = mobEvent.Description;
                    displayPartnerEvent.EventDate = mobEvent.EventDate;

                    displayPartnerEvents.Add(displayPartnerEvent);
                }
            }

            return Ok(displayPartnerEvents);
        }
    }
}
