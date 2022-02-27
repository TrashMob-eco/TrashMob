
namespace TrashMob.Controllers
{
    using System.Threading;
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
        public async Task<IActionResult> GetEventStatuses(CancellationToken cancellationToken)
        {
            var result = await eventStatusRepository.GetAllEventStatuses(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
