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
    [Route("api/partnerdocuments")]
    public class PartnerDocumentsController : BaseController
    {
        private readonly IBaseManager<PartnerDocument> manager;
        private readonly IUserRepository userRepository;

        public PartnerDocumentsController(IBaseManager<PartnerDocument> manager, 
                                             IUserRepository userRepository,
                                             TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommunityDocument(PartnerDocument partnerDocument)
        {
            var currentUser = await GetUser(userRepository);

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.Add(partnerDocument, currentUser.Id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddCommunityDocument));

            return Ok();
        }
    }
}
