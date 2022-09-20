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
    [Route("api/partnernotes")]
    public class PartnerNotesController : BaseController
    {
        private readonly IBaseManager<PartnerNote> manager;
        private readonly IUserRepository userRepository;

        public PartnerNotesController(TelemetryClient telemetryClient, 
                                      IUserRepository userRepository,
                                      IBaseManager<PartnerNote> manager)
            : base(telemetryClient, userRepository)
        {
            this.manager = manager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommunityNote(PartnerNote partnerNote)
        {
            var currentUser = await GetUser();

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.Add(partnerNote, currentUser.Id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddCommunityNote));

            return Ok();
        }
    }
}
