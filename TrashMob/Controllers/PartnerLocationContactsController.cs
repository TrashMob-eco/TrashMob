namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.Partners;

    [Authorize]
    [Route("api/partnerlocationcontacts")]
    public class PartnerLocationContactsController : SecureController
    {
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IBaseManager<PartnerLocationContact> partnerLocationContactManager;

        public PartnerLocationContactsController(IKeyedManager<Partner> partnerManager,
                                                 IBaseManager<PartnerLocationContact> partnerLocationContactManager)
            : base()
        {
            this.partnerManager = partnerManager;
            this.partnerLocationContactManager = partnerLocationContactManager;            
        }

        [HttpGet("{partnerLocationId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> Get(Guid partnerLocationId, CancellationToken cancellationToken)
        {
            var partnerLocationServices = await partnerLocationContactManager.GetByParentIdAsync(partnerLocationId, cancellationToken);

            return Ok(partnerLocationServices);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerLocationContact(PartnerLocationContact partnerLocationContact, CancellationToken cancellationToken = default)
        {
            var partner = partnerManager.GetAsync(partnerLocationContact.PartnerLocation.PartnerId, cancellationToken);
            
            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationContactManager.AddAsync(partnerLocationContact, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerLocationContact));

            return Ok();
        }
    }
}
