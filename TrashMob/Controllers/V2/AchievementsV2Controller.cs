namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Extensions.V2;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for achievement operations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/achievements")]
    public class AchievementsV2Controller(
        IAchievementManager achievementManager,
        ILogger<AchievementsV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets all available achievement types.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The list of achievement types.</returns>
        /// <response code="200">Returns the achievement types.</response>
        [HttpGet("types")]
        [ProducesResponseType(typeof(IReadOnlyList<AchievementTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAchievementTypes(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetAchievementTypes requested");

            var types = await achievementManager.GetAchievementTypesAsync(cancellationToken);
            var dtos = types.Select(t => t.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets a specific user's achievements (public profile view).
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user's achievements summary.</returns>
        /// <response code="200">Returns the user's achievements.</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(UserAchievementsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserAchievements(Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUserAchievements requested for User={UserId}", userId);

            var response = await achievementManager.GetUserAchievementsAsync(userId, cancellationToken);

            return Ok(response.ToV2Dto());
        }

        /// <summary>
        /// Gets the current authenticated user's achievements.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The current user's achievements summary.</returns>
        /// <response code="200">Returns the user's achievements.</response>
        /// <response code="401">Authentication required.</response>
        [HttpGet("my")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(UserAchievementsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyAchievements(CancellationToken cancellationToken)
        {
            var userId = new Guid(HttpContext.Items["UserId"].ToString());

            logger.LogInformation("V2 GetMyAchievements requested for User={UserId}", userId);

            var response = await achievementManager.GetUserAchievementsAsync(userId, cancellationToken);

            return Ok(response.ToV2Dto());
        }

        /// <summary>
        /// Gets the current user's unread (newly earned) achievement notifications.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The list of unread achievement notifications.</returns>
        /// <response code="200">Returns unread notifications.</response>
        /// <response code="401">Authentication required.</response>
        [HttpGet("my/unread")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<AchievementNotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUnreadAchievements(CancellationToken cancellationToken)
        {
            var userId = new Guid(HttpContext.Items["UserId"].ToString());

            logger.LogInformation("V2 GetUnreadAchievements requested for User={UserId}", userId);

            var notifications = await achievementManager.GetUnreadAchievementsAsync(userId, cancellationToken);
            var dtos = notifications.Select(n => n.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Marks achievements as read/notified for the current user.
        /// </summary>
        /// <param name="achievementTypeIds">The achievement type IDs to mark as read.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Achievements marked as read.</response>
        /// <response code="401">Authentication required.</response>
        [HttpPost("my/mark-read")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkAchievementsAsRead(
            [FromBody] IEnumerable<int> achievementTypeIds,
            CancellationToken cancellationToken)
        {
            var userId = new Guid(HttpContext.Items["UserId"].ToString());

            logger.LogInformation("V2 MarkAchievementsAsRead requested for User={UserId}", userId);

            await achievementManager.MarkAchievementsAsReadAsync(userId, achievementTypeIds, cancellationToken);

            return NoContent();
        }
    }
}
