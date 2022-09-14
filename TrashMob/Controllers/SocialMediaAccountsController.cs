namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [Authorize]
    [Route("api/socialmediaaccounts")]
    public class SocialMediaAccountController : BaseController
    {
        private readonly IExtendedManager<SocialMediaAccount> manager;
        private readonly IUserRepository userRepository;

        public SocialMediaAccountController(IExtendedManager<SocialMediaAccount> manager, 
                                          IUserRepository userRepository,
                                          TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddSocialMediaAccount(SocialMediaAccount socialMediaAccount)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.Add(socialMediaAccount, currentUser.Id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddSocialMediaAccount));

            return Ok();
        }
    }
}
