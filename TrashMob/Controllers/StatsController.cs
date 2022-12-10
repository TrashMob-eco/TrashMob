
namespace TrashMob.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Poco;
    using System.Threading;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Common;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/stats")]
    public class StatsController : BaseController
    {
        private readonly IEventManager eventManager;
        private readonly IEventSummaryManager eventSummaryManager;
        private readonly IEventAttendeeManager eventAttendeeManager;

        public StatsController(IEventManager eventManager,
                               IEventSummaryManager eventSummaryManager,
                               IEventAttendeeManager eventAttendeeManager)
            : base()
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
