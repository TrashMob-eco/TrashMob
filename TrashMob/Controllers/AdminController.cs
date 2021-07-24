namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using TrashMob.Shared;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IPartnerRequestRepository partnerRequestRepository;
        private readonly IUserRepository userRepository;

        public AdminController(IPartnerRequestRepository partnerRequestRepository, IUserRepository userRepository)
        {
            this.partnerRequestRepository = partnerRequestRepository;
            this.userRepository = userRepository;
        }

        [HttpPut("partnerrequestupdate/{userId}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdatePartnerRequest(Guid userId, PartnerRequest partnerRequest)
        {
            var user = await userRepository.GetUserByInternalId(userId).ConfigureAwait(false);

            if (!ValidateUser(user.NameIdentifier) || user.IsSiteAdmin)
            {
                return Forbid();
            }

            return Ok(await partnerRequestRepository.UpdatePartnerRequest(partnerRequest));
        }

        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
