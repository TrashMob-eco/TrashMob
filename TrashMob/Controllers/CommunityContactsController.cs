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
    [Route("api/communitycontacts")]
    public class CommunityContactsController : BaseController
    {
        private readonly IBaseManager<CommunityContact> manager;
        private readonly IUserRepository userRepository;

        public CommunityContactsController(IBaseManager<CommunityContact> manager, 
                                             IUserRepository userRepository,
                                             TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommunityContact(CommunityContact communityContact)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.Add(communityContact, currentUser.Id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddCommunityContact));

            return Ok();
        }
    }
}
