
namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/partnerrequeststatuses")]
    public class PartnerRequestStatusesController : BaseController
    {
        private readonly IPartnerRequestStatusRepository partnerRequestStatusRepository;

        public PartnerRequestStatusesController(TelemetryClient telemetryClient,
                                                IUserRepository userRepository,
                                                IPartnerRequestStatusRepository partnerRequestStatusRepository)
            : base(telemetryClient, userRepository)
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
