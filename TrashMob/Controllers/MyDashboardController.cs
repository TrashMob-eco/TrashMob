
namespace TrashMob.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Persistence;

    [Authorize]
    [RequiredScope("https://Trashmob.onmicrosoft.com/api/Trashmob.Read")] // The web API will only accept tokens 1) for users, and 2) having the "access_as_user" scope for this API
    [ApiController]
    [Route("api/mydashboard")]
    public class MyDashboardController : ControllerBase
    {
        private readonly IEventRepository eventRepository;

        public MyDashboardController(IEventRepository eventRepository)
        {
            this.eventRepository = eventRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyEvents()
        {
            var result = await eventRepository.GetAllEvents().ConfigureAwait(false);

            return Ok(result);
        }
    }
}
