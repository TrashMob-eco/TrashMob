using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TrashMob.Models.Poco;
namespace TrashMob.Controllers.V2;

using TrashMob.Shared.Managers.Interfaces;
/// <summary>
/// Controller for retrieving statistics about events and users.
/// </summary>
[ApiVersion("2.0")]
[ApiController]
[Route("api/v{version:apiVersion}")]
public class StatsControllerV2 : ControllerBase
{
    private readonly IEventSummaryManager _eventSummaryManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatsControllerV2"/> class.
    /// </summary>
    /// <param name="eventSummaryManager">event summary manager</param>
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
    public async Task<ActionResult<Stats>> GetStats(CancellationToken cancellationToken)
    {
        return Ok(await _eventSummaryManager.GetStatsV2Async(cancellationToken).ConfigureAwait(false));
    }
}
