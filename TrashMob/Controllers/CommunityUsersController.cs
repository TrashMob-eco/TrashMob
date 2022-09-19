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
    [Route("api/communityusers")]
    public class CommunityUsersController : BaseController
    {
        private readonly IBaseManager<CommunityUser> manager;
        private readonly IBaseManager<Community> communityManager;
        private readonly IUserRepository userRepository;

        public CommunityUsersController(IBaseManager<CommunityUser> manager,
                                        IBaseManager<Community> communityManager,
                                        IUserRepository userRepository,
                                        TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
            this.communityManager = communityManager;
            this.userRepository = userRepository;
        }

        [HttpGet("getcommunitiesforuser/{userId}")]
        public async Task<IActionResult> GetCommunitiesForUser(Guid userId, CancellationToken cancellationToken)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value, cancellationToken).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            var communities = await communityManager.GetByUserId(userId, cancellationToken);

            if (communities == null)
            {
                return NotFound();
            }

            return Ok(communities);
        }

        [HttpPost]
        public async Task<IActionResult> AddCommunityUser(CommunityUser communityUser)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.Add(communityUser, currentUser.Id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddCommunityUser));

            return Ok();
        }
    }
}
