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
    [Route("api/communitynotes")]
    public class CommunityNotesController : BaseController
    {
        private readonly IBaseManager<CommunityNote> manager;
        private readonly IUserRepository userRepository;

        public CommunityNotesController(IBaseManager<CommunityNote> manager, 
                                             IUserRepository userRepository,
                                             TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommunityNote(CommunityNote communityNote)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.Add(communityNote, currentUser.Id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddCommunityNote));

            return Ok();
        }
    }
}
