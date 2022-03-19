
namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;

    [Route("api/partnerstatuses")]
    public class PartnerStatusesController : BaseController
    {
        private readonly IPartnerStatusRepository partnerStatusRepository;

        public PartnerStatusesController(IPartnerStatusRepository partnerStatusRepository,
                                         TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.partnerStatusRepository = partnerStatusRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerStatuses(CancellationToken cancellationToken)
        {
            var result = await partnerStatusRepository.GetAllPartnerStatuses(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
