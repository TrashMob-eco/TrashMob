namespace TrashMob.Controllers.IFTTT
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco.IFTTT;

    [Route("api/ifttt/v1/[controller]")]
    [RequiredScope(Constants.TrashMobIFTTTScope)]
    [ApiController]
    public class QueriesController : SecureController
    {
        private readonly IQueriesManager queriesManager;

        public QueriesController(IQueriesManager queriesManager)
        {
            this.queriesManager = queriesManager;
        }

        [HttpPost("list_all_events")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [Authorize(Policy = AuthorizationPolicyConstants.IftttServiceKey)]
        public async Task<ActionResult> Get(QueriesRequest queriesRequest, CancellationToken cancellationToken)
        {
            var events = await queriesManager.GetEventsQueryDataAsync(queriesRequest, UserId, cancellationToken);

            var response = new DataResponse
            {
                Data = events,
            };

            return Ok(response);
        }
    }
}