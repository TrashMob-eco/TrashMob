namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for achievement operations.
    /// </summary>
    [Route("api/achievements")]
    public class AchievementsController(IAchievementManager achievementManager) : SecureController
    {
        /// <summary>
        /// Gets all available achievement types.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("types")]
        [ProducesResponseType(typeof(IEnumerable<AchievementType>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAchievementTypes(CancellationToken cancellationToken = default)
        {
            var types = await achievementManager.GetAchievementTypesAsync(cancellationToken);
            return Ok(types);
        }

        /// <summary>
        /// Gets the current user's achievements.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("my")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(UserAchievementsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyAchievements(CancellationToken cancellationToken = default)
        {
            var response = await achievementManager.GetUserAchievementsAsync(UserId, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Gets a specific user's achievements (public profile).
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(UserAchievementsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAchievements(Guid userId, CancellationToken cancellationToken = default)
        {
            var response = await achievementManager.GetUserAchievementsAsync(userId, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Gets the current user's unread (new) achievements.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("my/unread")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<NewAchievementNotification>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUnreadAchievements(CancellationToken cancellationToken = default)
        {
            var achievements = await achievementManager.GetUnreadAchievementsAsync(UserId, cancellationToken);
            return Ok(achievements);
        }

        /// <summary>
        /// Marks achievements as read/notified for the current user.
        /// </summary>
        /// <param name="achievementTypeIds">The achievement type IDs to mark as read.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("my/mark-read")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkAchievementsAsRead(
            [FromBody] IEnumerable<int> achievementTypeIds,
            CancellationToken cancellationToken = default)
        {
            await achievementManager.MarkAchievementsAsReadAsync(UserId, achievementTypeIds, cancellationToken);
            return NoContent();
        }
    }
}
