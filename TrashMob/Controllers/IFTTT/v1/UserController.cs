namespace TrashMob.Controllers.IFTTT
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/ifttt/v1/[controller]")]
    [ApiController]
    public class UserController : SecureController
    {
        private readonly IUserManager userManager;

        public UserController(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet("info")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<ActionResult> GetInfo(CancellationToken cancellationToken)
        {
            var user = await userManager.GetAsync(UserId, cancellationToken).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            var dataResponse = new DataResponse();

            var userResponse = new UserInfoResponse
            {
                Name = user.Email,
                Id = user.Id.ToString(),
            };

            dataResponse.Data = userResponse;

            return Ok(dataResponse);
        }
    }
}