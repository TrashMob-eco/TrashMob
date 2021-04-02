namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Persistence;

    [ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        private readonly IMapRepository mapRepository;

        public MapsController(IMapRepository mapRepository)
        {
            this.mapRepository = mapRepository;
        }

        [HttpGet]
        public IActionResult GetMapKey()
        {
            return Ok(mapRepository.GetMapKey());
        }
    }
}
