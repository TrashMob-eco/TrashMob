namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Authorize]
    [Route("api/partnerdocuments")]
    public class PartnerDocumentsController : BaseController
    {
        private readonly IBaseManager<PartnerDocument> manager;
        private readonly IUserRepository userRepository;

        public PartnerDocumentsController(TelemetryClient telemetryClient,
                                             IUserRepository userRepository, 
                                             IBaseManager<PartnerDocument> manager)
            : base(telemetryClient, userRepository)
        {
            this.manager = manager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommunityDocument(PartnerDocument partnerDocument)
        {
            var currentUser = await GetUser();

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
