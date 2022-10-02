namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partnercontacts")]
    public class PartnerContactsController : SecureController
    {
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IBaseManager<PartnerContact> manager;

        public PartnerContactsController(IKeyedManager<Partner> partnerManager,
                                         IBaseManager<PartnerContact> manager)
            : base()
        {
            this.partnerManager = partnerManager;
            this.manager = manager;            
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerContact(PartnerContact partnerContact)
        {
            var partner = partnerManager.Get(partnerContact.PartnerId);
            
            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await manager.Add(partnerContact, UserId).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerContact));

            return Ok();
        }
    }
}
