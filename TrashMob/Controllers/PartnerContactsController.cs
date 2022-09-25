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
    [Route("api/partnercontacts")]
    public class PartnerContactsController : BaseController
    {
        private readonly IBaseManager<PartnerContact> manager;
        private readonly IUserRepository userRepository;

        public PartnerContactsController(TelemetryClient telemetryClient,
                                         IUserRepository userRepository,
                                         IBaseManager<PartnerContact> manager)
            : base(telemetryClient, userRepository)
        {
            this.manager = manager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommunityContact(PartnerContact partnerContact)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            await manager.Add(partnerContact, currentUser.Id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddCommunityContact));

            return Ok();
        }
    }
}
