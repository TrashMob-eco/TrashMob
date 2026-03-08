namespace TrashMob.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for users with server-side pagination and filtering.
    /// Returns PII-safe UserDto (no email, identity provider fields, or date of birth).
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersV2Controller(
        IUserManager userManager,
        ILogger<UsersV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets a paginated list of users with optional filtering.
        /// </summary>
        /// <param name="filter">Query parameters for pagination and filtering.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of users.</returns>
        /// <response code="200">Returns the paginated user list.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] UserQueryParameters filter,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUsers requested with Page={Page}, PageSize={PageSize}",
                filter.Page, filter.PageSize);

            var query = userManager.GetFilteredUsersQueryable(filter);
            var result = await query.ToPagedAsync(filter, u => u.ToV2Dto(), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Gets a single user by their identifier.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user details (PII-safe).</returns>
        /// <response code="200">Returns the user.</response>
        /// <response code="404">User not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUser(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUser requested for {UserId}", id);

            var user = await userManager.GetAsync(id, cancellationToken);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user.ToV2Dto());
        }
    }
}
