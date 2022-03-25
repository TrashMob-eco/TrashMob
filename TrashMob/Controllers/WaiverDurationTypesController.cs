
namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;

    [Route("api/waiverdurationtypes")]
    public class WaiverDurationTypesController : BaseController
    {
        private readonly IWaiverDurationTypeRepository waiverDurationTypeRepository;

        public WaiverDurationTypesController(IWaiverDurationTypeRepository waiverDurationTypeRepository,
                                             TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.waiverDurationTypeRepository = waiverDurationTypeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetWaiverDurationTypes(CancellationToken cancellationToken)
        {
            var result = await waiverDurationTypeRepository.GetAllWaiverDurationTypes(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
