
namespace TrashMob.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/partnerstatuses")]
    public class PartnerStatusesController : ControllerBase
    {
        private readonly IPartnerStatusRepository partnerStatusRepository;

        public PartnerStatusesController(IPartnerStatusRepository partnerStatusRepository)
        {
            this.partnerStatusRepository = partnerStatusRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerStatuses()
        {
            var result = await partnerStatusRepository.GetAllPartnerStatuses().ConfigureAwait(false);
            return Ok(result);
        }
    }
}
