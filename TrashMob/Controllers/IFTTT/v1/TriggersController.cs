namespace TrashMob.Controllers.IFTTT
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Security;

    [Route("api/ifttt/v1/[controller]")]
    [ApiController]
    public class TriggersController : SecureController
    {
        public TriggersController() 
        {
        }

        [HttpPost("NewEvents")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<ActionResult> Get(TriggersRequest triggersRequest, CancellationToken cancellationToken)
        {
            var dataResponse = new DataResponse();

            return Ok(dataResponse);
        }
    }
}
