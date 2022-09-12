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
    [Route("api/communityrequest")]
    public class CommunityRequestController : BaseController
    {
        private readonly IExtendedManager<CommunityRequest> manager;
        private readonly IUserRepository userRepository;

        public CommunityRequestController(IExtendedManager<CommunityRequest> manager, 
                                          IUserRepository userRepository,
                                          TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> SaveCommunityRequest(CommunityRequest communityRequest)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.Add(communityRequest, currentUser.Id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(SaveCommunityRequest));

            return Ok();
        }
    }
}
