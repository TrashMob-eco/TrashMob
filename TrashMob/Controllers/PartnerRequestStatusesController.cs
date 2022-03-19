
namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;

    [Route("api/partnerrequeststatuses")]
    public class PartnerRequestStatusesController : BaseController
    {
        private readonly IPartnerRequestStatusRepository partnerRequestStatusRepository;

        public PartnerRequestStatusesController(IPartnerRequestStatusRepository partnerRequestStatusRepository,
                                                TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.partnerRequestStatusRepository = partnerRequestStatusRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerRequestStatuses(CancellationToken cancellationToken)
        {
            var result = await partnerRequestStatusRepository.GetAllPartnerRequestStatuses(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
