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

    /// <summary>
    /// Controller for IFTTT queries, providing endpoints for listing all events.
    /// </summary>
    [Route("api/ifttt/v1/[controller]")]
    [RequiredScope(Constants.TrashMobIFTTTScope)]
    public class QueriesController(IQueriesManager queriesManager)
        : SecureController
    {

        /// <summary>
        /// Gets all events for IFTTT queries.
        /// </summary>
        /// <param name="queriesRequest">The queries request payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result with event data.</remarks>
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