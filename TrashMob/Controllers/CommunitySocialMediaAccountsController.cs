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
    [Route("api/communitysocialmediaaccounts")]
    public class CommunitySocialMediaAccountController : BaseController
    {
        private readonly IBaseManager<CommunitySocialMediaAccount> manager;
        private readonly IUserRepository userRepository;

        public CommunitySocialMediaAccountController(IBaseManager<CommunitySocialMediaAccount> manager, 
                                          IUserRepository userRepository,
                                          TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommunitySocialMediaAccount(CommunitySocialMediaAccount communitySocialMediaAccount)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.Add(communitySocialMediaAccount, currentUser.Id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddCommunitySocialMediaAccount));

            return Ok();
        }
    }
}
