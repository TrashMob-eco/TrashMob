namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for retrieving statistics about events and users.
    /// </summary>
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/stats")]
    public class StatsController(IEventSummaryManager eventSummaryManager) : BaseController
    {
        /// <summary>
        /// Gets overall event statistics.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Overall event statistics.</remarks>
        [HttpGet]
        public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
        {
            var stats = await eventSummaryManager.GetStatsAsync(cancellationToken);

            return Ok(stats);
        }

        /// <summary>
        /// Gets event statistics for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>User-specific event statistics.</remarks>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetStatsByUser(Guid userId, CancellationToken cancellationToken)
        {
            var stats = await eventSummaryManager.GetStatsByUser(userId, cancellationToken);

            return Ok(stats);
        }
    }
}