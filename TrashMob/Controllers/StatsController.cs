namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Managers.Interfaces;

    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/stats")]
    public class StatsController(IEventSummaryManager eventSummaryManager) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
        {
            var stats = await eventSummaryManager.GetStatsAsync(cancellationToken);

            return Ok(stats);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetStatsByUser(Guid userId, CancellationToken cancellationToken)
        {
            var stats = await eventSummaryManager.GetStatsByUser(userId, cancellationToken);

            return Ok(stats);
        }
    }
}