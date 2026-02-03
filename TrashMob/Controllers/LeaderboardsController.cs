namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for leaderboard operations.
    /// </summary>
    [Route("api/leaderboards")]
    public class LeaderboardsController(ILeaderboardManager leaderboardManager) : SecureController
    {
        /// <summary>
        /// Gets the leaderboard for the specified parameters.
        /// </summary>
        /// <param name="type">The leaderboard type (Events, Bags, Weight, Hours). Default: Events.</param>
        /// <param name="timeRange">The time range (Week, Month, Year, AllTime). Default: Month.</param>
        /// <param name="scope">The location scope (Global, Region, City). Default: Global.</param>
        /// <param name="location">The location value (required if scope is not Global).</param>
        /// <param name="limit">Maximum entries to return (1-100). Default: 50.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(LeaderboardResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLeaderboard(
            [FromQuery] string type = "Events",
            [FromQuery] string timeRange = "Month",
            [FromQuery] string scope = "Global",
            [FromQuery] string location = null,
            [FromQuery] int limit = 50,
            CancellationToken cancellationToken = default)
        {
            var response = await leaderboardManager.GetLeaderboardAsync(
                type,
                timeRange,
                scope,
                location,
                limit,
                cancellationToken);

            return Ok(response);
        }

        /// <summary>
        /// Gets the current user's rank on a leaderboard.
        /// </summary>
        /// <param name="type">The leaderboard type. Default: Events.</param>
        /// <param name="timeRange">The time range. Default: AllTime.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("my-rank")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(UserRankResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyRank(
            [FromQuery] string type = "Events",
            [FromQuery] string timeRange = "AllTime",
            CancellationToken cancellationToken = default)
        {
            var response = await leaderboardManager.GetUserRankAsync(
                UserId,
                type,
                timeRange,
                cancellationToken);

            return Ok(response);
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
        [HttpGet("teams")]
        [ProducesResponseType(typeof(LeaderboardResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeamLeaderboard(
            [FromQuery] string type = "Events",
            [FromQuery] string timeRange = "Month",
            [FromQuery] string scope = "Global",
            [FromQuery] string location = null,
            [FromQuery] int limit = 50,
            CancellationToken cancellationToken = default)
        {
            var response = await leaderboardManager.GetTeamLeaderboardAsync(
                type,
                timeRange,
                scope,
                location,
                limit,
                cancellationToken);

            return Ok(response);
        }

        /// <summary>
        /// Gets a specific team's rank on a leaderboard.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="type">The leaderboard type. Default: Events.</param>
        /// <param name="timeRange">The time range. Default: AllTime.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("teams/{teamId}/rank")]
        [ProducesResponseType(typeof(TeamRankResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeamRank(
            Guid teamId,
            [FromQuery] string type = "Events",
            [FromQuery] string timeRange = "AllTime",
            CancellationToken cancellationToken = default)
        {
            var response = await leaderboardManager.GetTeamRankAsync(
                teamId,
                type,
                timeRange,
                cancellationToken);

            if (response.IneligibleReason == "Team not found.")
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Gets available leaderboard options (types, time ranges, scopes).
        /// </summary>
        [HttpGet("options")]
        [ProducesResponseType(typeof(LeaderboardOptions), StatusCodes.Status200OK)]
        public IActionResult GetOptions()
        {
            return Ok(new LeaderboardOptions
            {
                Types = leaderboardManager.GetAvailableLeaderboardTypes(),
                TimeRanges = leaderboardManager.GetAvailableTimeRanges(),
                LocationScopes = leaderboardManager.GetAvailableLocationScopes()
            });
        }
    }

    /// <summary>
    /// Available options for leaderboard queries.
    /// </summary>
    public class LeaderboardOptions
    {
        /// <summary>
        /// Gets or sets available leaderboard types.
        /// </summary>
        public string[] Types { get; set; }

        /// <summary>
        /// Gets or sets available time ranges.
        /// </summary>
        public string[] TimeRanges { get; set; }

        /// <summary>
        /// Gets or sets available location scopes.
        /// </summary>
        public string[] LocationScopes { get; set; }
    }
}
