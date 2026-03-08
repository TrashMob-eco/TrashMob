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
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for retrieving platform and user statistics.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/stats")]
    public class StatsV2Controller(
        IEventSummaryManager eventSummaryManager,
        ILogger<StatsV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets overall platform statistics.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Platform-wide statistics.</returns>
        /// <response code="200">Returns the platform statistics.</response>
        [HttpGet]
        [ProducesResponseType(typeof(StatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetStats requested");
            var stats = await eventSummaryManager.GetStatsAsync(cancellationToken);
            return Ok(stats.ToV2Dto());
        }

        /// <summary>
        /// Gets statistics for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User-specific statistics.</returns>
        /// <response code="200">Returns the user's statistics.</response>
        /// <response code="404">User not found.</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(StatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStatsByUser(Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetStatsByUser requested for {UserId}", userId);
            var stats = await eventSummaryManager.GetStatsByUser(userId, cancellationToken);
            return Ok(stats.ToV2Dto());
        }
    }
}
