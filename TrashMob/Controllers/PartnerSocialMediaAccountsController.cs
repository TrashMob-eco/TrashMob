namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    [Authorize]
    [Route("api/partnersocialmediaaccounts")]
    public class PartnerSocialMediaAccountController : BaseController
    {
        private readonly IBaseManager<PartnerSocialMediaAccount> manager;
        private readonly IUserRepository userRepository;

        public PartnerSocialMediaAccountController(IBaseManager<PartnerSocialMediaAccount> manager, 
                                          IUserRepository userRepository,
                                          TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerSocialMediaAccount(PartnerSocialMediaAccount partnerSocialMediaAccount)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.Add(partnerSocialMediaAccount, currentUser.Id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerSocialMediaAccount));

            return Ok();
        }
    }
}
