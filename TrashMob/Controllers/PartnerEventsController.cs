
namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partnerevents")]
    public class PartnerEventsController : SecureController
    {
        private readonly IEventPartnerManager eventPartnerManager;
        private readonly IKeyedManager<PartnerLocation> partnerLocationManager;
        private readonly IKeyedManager<Event> eventManager;
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnerEventsController(IEventPartnerManager eventPartnerManager,
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
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var events = await eventPartnerManager.GetByPartnerIdAsync(partnerId, cancellationToken).ConfigureAwait(false);

            return Ok(events);
        }
    }
}
