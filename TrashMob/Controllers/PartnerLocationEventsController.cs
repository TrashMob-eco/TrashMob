
namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partnerlocationevents")]
    public class PartnerLocationEventsController : SecureController
    {
        private readonly IEventPartnerLocationManager eventPartnerLocationManager;
        private readonly IPartnerLocationManager partnerLocationManager;

        public PartnerLocationEventsController(IEventPartnerLocationManager eventPartnerLocationManager,
                                               IPartnerLocationManager partnerLocationManager)
            : base()
        {
            this.eventPartnerLocationManager = eventPartnerLocationManager;
            this.partnerLocationManager = partnerLocationManager;
        }

        [HttpGet("{partnerLocationId}")]
        public async Task<IActionResult> GetPartnerLocationEvents(Guid partnerLocationId, CancellationToken cancellationToken)
        {
            var partner = await partnerLocationManager.GetPartnerForLocation(partnerLocationId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var events = await eventPartnerLocationManager.GetByPartnerLocationIdAsync(partnerLocationId, cancellationToken).ConfigureAwait(false);

            return Ok(events);
        }
    }
}
