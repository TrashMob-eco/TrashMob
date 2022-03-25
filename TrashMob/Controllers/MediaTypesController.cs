
namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Persistence;

    [Route("api/mediatypes")]
    public class MediaTypesController : BaseController
    {
        private readonly IMediaTypeRepository mediaTypeRepository;

        public MediaTypesController(IMediaTypeRepository mediaTypeRepository,
                                    TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.mediaTypeRepository = mediaTypeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetMediaTypes(CancellationToken cancellationToken)
        {
            var result = await mediaTypeRepository.GetAllMediaTypes(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
