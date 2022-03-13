namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/waivers")]
    public class WaiversController : ControllerBase
    {
        private readonly IWaiverRepository waiverRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserWaiverRepository userWaiverRepository;

        public WaiversController(IWaiverRepository waiverRepository, IUserRepository userRepository, IUserWaiverRepository userWaiverRepository)
        {
            this.waiverRepository = waiverRepository;
            this.userRepository = userRepository;
            this.userWaiverRepository = userWaiverRepository;
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

            return Ok(await waiverRepository.AddWaiver(waiver).ConfigureAwait(false));
        }
    }
}
