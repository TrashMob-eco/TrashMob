namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partnerlocationcontacts")]
    public class PartnerLocationContactsController : SecureController
    {
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IBaseManager<PartnerLocationContact> manager;

        public PartnerLocationContactsController(IKeyedManager<Partner> partnerManager,
                                                 IBaseManager<PartnerLocationContact> manager)
            : base()
        {
            this.partnerManager = partnerManager;
            this.manager = manager;            
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerLocationContact(PartnerLocationContact partnerLocationContact, CancellationToken cancellationToken = default)
        {
            var partner = partnerManager.GetAsync(partnerLocationContact.PartnerId, cancellationToken);
            
            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await manager.AddAsync(partnerLocationContact, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerLocationContact));

            return Ok();
        }
    }
}
