namespace TrashMob.Controllers.V2
{
    using System;
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
    using TrashMob.Shared.Extensions.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for leaderboard operations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/leaderboards")]
    public class LeaderboardsV2Controller(
        ILeaderboardManager leaderboardManager,
        ILogger<LeaderboardsV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets the user leaderboard for the specified parameters.
        /// </summary>
        /// <param name="type">The leaderboard type (Events, Bags, Weight, Hours). Default: Events.</param>
        /// <param name="timeRange">The time range (Week, Month, Year, AllTime). Default: Month.</param>
        /// <param name="scope">The location scope (Global, Region, City). Default: Global.</param>
        /// <param name="location">The location value (required if scope is not Global).</param>
        /// <param name="limit">Maximum entries to return (1-100). Default: 50.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The leaderboard response.</returns>
        /// <response code="200">Returns the leaderboard.</response>
        [HttpGet]
        [ProducesResponseType(typeof(LeaderboardResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLeaderboard(
            [FromQuery] string type = "Events",
            [FromQuery] string timeRange = "Month",
            [FromQuery] string scope = "Global",
            [FromQuery] string location = null,
            [FromQuery] int limit = 50,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetLeaderboard requested Type={Type}, TimeRange={TimeRange}, Scope={Scope}, Limit={Limit}",
                type, timeRange, scope, limit);

            var response = await leaderboardManager.GetLeaderboardAsync(
                type, timeRange, scope, location, limit, cancellationToken);

            return Ok(response.ToV2Dto());
        }

        /// <summary>
        /// Gets the team leaderboard for the specified parameters.
        /// </summary>
        /// <param name="type">The leaderboard type (Events, Bags, Weight, Hours). Default: Events.</param>
        /// <param name="timeRange">The time range (Week, Month, Year, AllTime). Default: Month.</param>
        /// <param name="scope">The location scope (Global, Region, City). Default: Global.</param>
        /// <param name="location">The location value (required if scope is not Global).</param>
        /// <param name="limit">Maximum entries to return (1-100). Default: 50.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The team leaderboard response.</returns>
        /// <response code="200">Returns the team leaderboard.</response>
        [HttpGet("teams")]
        [ProducesResponseType(typeof(LeaderboardResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTeamLeaderboard(
            [FromQuery] string type = "Events",
            [FromQuery] string timeRange = "Month",
            [FromQuery] string scope = "Global",
            [FromQuery] string location = null,
            [FromQuery] int limit = 50,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetTeamLeaderboard requested Type={Type}, TimeRange={TimeRange}, Scope={Scope}, Limit={Limit}",
                type, timeRange, scope, limit);

            var response = await leaderboardManager.GetTeamLeaderboardAsync(
                type, timeRange, scope, location, limit, cancellationToken);

            return Ok(response.ToV2Dto());
        }

        /// <summary>
        /// Gets a specific team's rank on a leaderboard.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="type">The leaderboard type. Default: Events.</param>
        /// <param name="timeRange">The time range. Default: AllTime.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The team's rank information.</returns>
        /// <response code="200">Returns the team's rank.</response>
        /// <response code="404">Team not found.</response>
        [HttpGet("teams/{teamId}/rank")]
        [ProducesResponseType(typeof(TeamRankDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTeamRank(
            Guid teamId,
            [FromQuery] string type = "Events",
            [FromQuery] string timeRange = "AllTime",
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetTeamRank requested Team={TeamId}, Type={Type}, TimeRange={TimeRange}",
                teamId, type, timeRange);

            var response = await leaderboardManager.GetTeamRankAsync(
                teamId, type, timeRange, cancellationToken);

            if (response.IneligibleReason == "Team not found.")
            {
                return NotFound(response.ToV2Dto());
            }

            return Ok(response.ToV2Dto());
        }

        /// <summary>
        /// Gets the current authenticated user's rank on a leaderboard.
        /// </summary>
        /// <param name="type">The leaderboard type. Default: Events.</param>
        /// <param name="timeRange">The time range. Default: AllTime.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user's rank information.</returns>
        /// <response code="200">Returns the user's rank.</response>
        /// <response code="401">Authentication required.</response>
        [HttpGet("my-rank")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(UserRankDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyRank(
            [FromQuery] string type = "Events",
            [FromQuery] string timeRange = "AllTime",
            CancellationToken cancellationToken = default)
        {
            var userId = new Guid(HttpContext.Items["UserId"].ToString());

            logger.LogInformation("V2 GetMyRank requested User={UserId}, Type={Type}, TimeRange={TimeRange}",
                userId, type, timeRange);

            var response = await leaderboardManager.GetUserRankAsync(
                userId, type, timeRange, cancellationToken);

            return Ok(response.ToV2Dto());
        }

        /// <summary>
        /// Gets the available leaderboard options (types, time ranges, scopes).
        /// </summary>
        /// <returns>Available leaderboard options.</returns>
        /// <response code="200">Returns the available options.</response>
        [HttpGet("options")]
        [ProducesResponseType(typeof(LeaderboardOptionsDto), StatusCodes.Status200OK)]
        public IActionResult GetOptions()
        {
            logger.LogInformation("V2 GetOptions requested");

            return Ok(new LeaderboardOptionsDto
            {
                Types = leaderboardManager.GetAvailableLeaderboardTypes(),
                TimeRanges = leaderboardManager.GetAvailableTimeRanges(),
                LocationScopes = leaderboardManager.GetAvailableLocationScopes(),
            });
        }
    }
}
