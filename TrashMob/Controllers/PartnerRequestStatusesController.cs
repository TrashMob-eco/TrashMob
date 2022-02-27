
namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/partnerrequeststatuses")]
    public class PartnerRequestStatusesController : ControllerBase
    {
        private readonly IPartnerRequestStatusRepository partnerRequestStatusRepository;

        public PartnerRequestStatusesController(IPartnerRequestStatusRepository partnerRequestStatusRepository)
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
