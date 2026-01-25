using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace TrashMob.Controllers.V2;

[ApiVersion("2.0")]
[ApiController]
[Route("api/v{version:apiVersion}")]
public class StatsControllerV2 : ControllerBase
{
    public StatsControllerV2()
    {
        
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        return Ok();
    }
}
