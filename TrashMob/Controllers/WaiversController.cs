namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [Route("api/waivers")]
    public class WaiversController : BaseController
    {
        private readonly IWaiverRepository waiverRepository;
        private readonly IUserRepository userRepository;

        public WaiversController(IWaiverRepository waiverRepository,
                                 IUserRepository userRepository,
                                 TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.waiverRepository = waiverRepository;
            this.userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetWaivers(CancellationToken cancellationToken)
        {
            return Ok(await waiverRepository.GetWaivers(cancellationToken).ConfigureAwait(false));
        }

        [HttpGet("{waiverId}")]
        public async Task<IActionResult> GetWaiver(Guid waiverId, CancellationToken cancellationToken)
        {
            return Ok(await waiverRepository.GetWaiver(waiverId, cancellationToken).ConfigureAwait(false));
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateWaiver(Waiver waiver)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            // Ensure user is allowed to update this Partner
            if (!currentUser.IsSiteAdmin)
            {
                return Forbid();
            }

            TelemetryClient.TrackEvent(nameof(UpdateWaiver));

            return Ok(await waiverRepository.UpdateWaiver(waiver).ConfigureAwait(false));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddWaiver(Waiver waiver)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            // Ensure user is allowed to update this Partner
            if (!currentUser.IsSiteAdmin)
            {
                return Forbid();
            }

            TelemetryClient.TrackEvent(nameof(AddWaiver));
            return Ok(await waiverRepository.AddWaiver(waiver).ConfigureAwait(false));
        }
    }
}
