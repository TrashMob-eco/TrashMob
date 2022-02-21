
namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/waiverdurationtypes")]
    public class WaiverDurationTypesController : ControllerBase
    {
        private readonly IWaiverDurationTypeRepository waiverDurationTypeRepository;

        public WaiverDurationTypesController(IWaiverDurationTypeRepository waiverDurationTypeRepository)
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
