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
    [Route("api/communities")]
    public class CommunitiesController : BaseController
    {
        private readonly IBaseManager<Community> manager;
        private readonly IUserRepository userRepository;

        public CommunitiesController(IKeyedManager<Community> manager, 
                                     IUserRepository userRepository,
                                     TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommunity(Community community)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.Add(community, currentUser.Id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddCommunity));

            return Ok();
        }
    }
}
