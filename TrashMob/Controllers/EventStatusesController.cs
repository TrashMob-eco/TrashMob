
namespace TrashMob.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/eventstatuses")]
    public class EventsStatusesController : ControllerBase
    {
        private readonly IEventStatusRepository eventStatusRepository;

        public EventsStatusesController(IEventStatusRepository eventStatusRepository)
        {
            this.eventStatusRepository = eventStatusRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventStatuses()
        {
            var result = await eventStatusRepository.GetAllEventStatuses().ConfigureAwait(false);
            return Ok(result);
        }
    }
}
