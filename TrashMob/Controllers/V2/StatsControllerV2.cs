using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TrashMob.Shared.Managers.Interfaces;

namespace TrashMob.Controllers.V2;

[ApiVersion("2.0")]
[ApiController]
[Route("api/v{version:apiVersion}")]
public class StatsControllerV2 : ControllerBase
{
    private readonly IEventSummaryManager _eventSummaryManager;

    public StatsControllerV2(IEventSummaryManager eventSummaryManager)
    {
        _eventSummaryManager = eventSummaryManager;
    }

    /// <summary>
    /// Gets overall event statistics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>Overall event statistics.</remarks>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        return Ok(await _eventSummaryManager.GetStatsV2Async(cancellationToken).ConfigureAwait(false));
    }
}
