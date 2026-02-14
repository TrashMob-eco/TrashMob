namespace TrashMob.Controllers.IFTTT
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for IFTTT user info, providing endpoints for user information retrieval.
    /// </summary>
    [Route("api/ifttt/v1/[controller]")]
    public class UserController : SecureController
    {
        private readonly IUserManager userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        public UserController(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// Gets user info for IFTTT integration.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result with user info.</remarks>
        [HttpGet("info")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<ActionResult> GetInfo(CancellationToken cancellationToken)
        {
            var user = await userManager.GetAsync(UserId, cancellationToken);

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