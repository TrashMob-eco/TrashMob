
namespace TrashMob.Controllers
{
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
        public async Task<IActionResult> GetPartnerRequestStatuses()
        {
            var result = await partnerRequestStatusRepository.GetAllPartnerRequestStatuses().ConfigureAwait(false);
            return Ok(result);
        }
    }
}
