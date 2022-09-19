namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [Authorize]
    [Route("api/communityrequests")]
    public class CommunityRequestsController : BaseController
    {
        private readonly IBaseManager<CommunityRequest> manager;
        private readonly IUserRepository userRepository;

        public CommunityRequestsController(IBaseManager<CommunityRequest> manager, 
                                          IUserRepository userRepository,
                                          TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommunityRequest(CommunityRequest communityRequest)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.Add(communityRequest, currentUser.Id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddCommunityRequest));

            return Ok();
        }

        [HttpGet("getcommunityrequestsforuser/{userId}")]
        public async Task<IActionResult> GetCommunityRequestsForUser(Guid userId, CancellationToken cancellationToken)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value, cancellationToken).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.GetByUserId(userId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(GetCommunityRequestsForUser));

            return Ok();
        }
    }
}
