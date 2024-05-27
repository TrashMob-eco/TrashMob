namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/stats")]
    public class StatsController : BaseController
    {
        private readonly IEventAttendeeManager eventAttendeeManager;
        private readonly IEventManager eventManager;
        private readonly IEventSummaryManager eventSummaryManager;

        public StatsController(IEventManager eventManager,
            IEventSummaryManager eventSummaryManager,
            IEventAttendeeManager eventAttendeeManager)
        {
            this.eventManager = eventManager;
            this.eventSummaryManager = eventSummaryManager;
            this.eventAttendeeManager = eventAttendeeManager;
        }

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