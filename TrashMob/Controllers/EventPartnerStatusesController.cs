
namespace TrashMob.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/eventpartnerstatuses")]
    public class EventPartnerStatusesController : ControllerBase
    {
        private readonly IEventPartnerStatusRepository eventPartnerStatusRepository;

        public EventPartnerStatusesController(IEventPartnerStatusRepository eventPartnerStatusRepository)
        {
            this.eventPartnerStatusRepository = eventPartnerStatusRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventPartnerStatuses()
        {
            var result = await eventPartnerStatusRepository.GetAllEventPartnerStatuses().ConfigureAwait(false);
            return Ok(result);
        }
    }
}
