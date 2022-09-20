
namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Poco;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/partnerevents")]
    public class PartnerEventsController : BaseController
    {
        private readonly IEventPartnerRepository eventPartnerRepository;
        private readonly IUserRepository userRepository;
        private readonly IPartnerRepository partnerRepository;
        private readonly IPartnerLocationRepository partnerLocationRepository;
        private readonly IEventRepository eventRepository;
        private readonly IPartnerUserRepository partnerUserRepository;

        public PartnerEventsController(IEventPartnerRepository eventPartnerRepository, 
                                       IUserRepository userRepository, 
                                       IPartnerRepository partnerRepository, 
                                       IPartnerLocationRepository partnerLocationRepository, 
                                       IEventRepository eventRepository,
                                       IPartnerUserRepository partnerUserRepository,
                                       TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.eventPartnerRepository = eventPartnerRepository;
            this.userRepository = userRepository;
            this.partnerRepository = partnerRepository;
            this.partnerLocationRepository = partnerLocationRepository;
            this.eventRepository = eventRepository;
            this.partnerUserRepository = partnerUserRepository;
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetPartnerEvents(Guid partnerId, CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value, cancellationToken).ConfigureAwait(false);

            if (!currentUser.IsSiteAdmin)
            {
                var currentUserPartner = partnerUserRepository.GetPartnerUsers(cancellationToken).FirstOrDefault(pu => pu.PartnerId == partnerId && pu.UserId == currentUser.Id);

                if (currentUserPartner == null)
                {
                    return Forbid();
                }
            }

            var displayPartnerEvents = new List<DisplayPartnerEvent>();
            var currentPartners = await eventPartnerRepository.GetPartnerEvents(partnerId, cancellationToken).ConfigureAwait(false);

            if (currentPartners.Any())
            {
                var partner = await partnerRepository.GetPartner(partnerId, cancellationToken).ConfigureAwait(false);

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

                    var partnerLocation = partnerLocationRepository.GetPartnerLocations(cancellationToken).FirstOrDefault(pl => pl.PartnerId == cp.PartnerId && pl.Id == cp.PartnerLocationId);

                    displayPartnerEvent.PartnerLocationName = partnerLocation.Name;

                    var mobEvent = await eventRepository.GetEvent(cp.EventId, cancellationToken).ConfigureAwait(false);

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
