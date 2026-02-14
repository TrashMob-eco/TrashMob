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
    /// Controller for IFTTT triggers, providing endpoints for event creation triggers by location and type.
    /// </summary>
    [Route("api/ifttt/v1/[controller]")]
    [RequiredScope(Constants.TrashMobIFTTTScope)]
    public class TriggersController(ITriggersManager triggersManager)
        : SecureController
    {

        /// <summary>
        /// Gets all new events created (IFTTT trigger).
        /// </summary>
        /// <param name="triggersRequest">The triggers request payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result with event data.</remarks>
        [HttpPost("any_new_event_created")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<ActionResult> Get(TriggersRequest triggersRequest, CancellationToken cancellationToken)
        {
            var error = triggersManager.ValidateRequest(triggersRequest, EventRequestType.All);

            if (error is not null)
            {
                return BadRequest(error);
            }

            var events = await triggersManager.GetEventsTriggerDataAsync(triggersRequest, UserId, cancellationToken);

            var response = new DataResponse
            {
                Data = events,
            };

            return Ok(response);
        }

        /// <summary>
        /// Gets new events created by country (IFTTT trigger).
        /// </summary>
        /// <param name="triggersRequest">The triggers request payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result with event data.</remarks>
        [HttpPost("new_event_created_by_country")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<ActionResult> GetByCountry(TriggersRequest triggersRequest,
            CancellationToken cancellationToken)
        {
            var error = triggersManager.ValidateRequest(triggersRequest, EventRequestType.ByCountry);

            if (error is not null)
            {
                return BadRequest(error);
            }

            var events = await triggersManager.GetEventsTriggerDataAsync(triggersRequest, UserId, cancellationToken);

            var response = new DataResponse
            {
                Data = events,
            };

            return Ok(response);
        }

        /// <summary>
        /// Gets new events created by region (IFTTT trigger).
        /// </summary>
        /// <param name="triggersRequest">The triggers request payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result with event data.</remarks>
        [HttpPost("new_event_created_by_region")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<ActionResult> GetByRegion(TriggersRequest triggersRequest,
            CancellationToken cancellationToken)
        {
            var error = triggersManager.ValidateRequest(triggersRequest, EventRequestType.ByRegion);

            if (error is not null)
            {
                return BadRequest(error);
            }

            var events = await triggersManager.GetEventsTriggerDataAsync(triggersRequest, UserId, cancellationToken);

            var response = new DataResponse
            {
                Data = events,
            };

            return Ok(response);
        }

        /// <summary>
        /// Gets new events created by city (IFTTT trigger).
        /// </summary>
        /// <param name="triggersRequest">The triggers request payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result with event data.</remarks>
        [HttpPost("new_event_created_by_city")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<ActionResult> GetByCity(TriggersRequest triggersRequest, CancellationToken cancellationToken)
        {
            var error = triggersManager.ValidateRequest(triggersRequest, EventRequestType.ByCity);

            if (error is not null)
            {
                return BadRequest(error);
            }

            var events = await triggersManager.GetEventsTriggerDataAsync(triggersRequest, UserId, cancellationToken);

            var response = new DataResponse
            {
                Data = events,
            };

            return Ok(response);
        }

        /// <summary>
        /// Gets new events created by postal code (IFTTT trigger).
        /// </summary>
        /// <param name="triggersRequest">The triggers request payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result with event data.</remarks>
        [HttpPost("new_event_created_by_postal_code")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<ActionResult> GetByPostalCode(TriggersRequest triggersRequest,
            CancellationToken cancellationToken)
        {
            var error = triggersManager.ValidateRequest(triggersRequest, EventRequestType.ByPostalCode);

            if (error is not null)
            {
                return BadRequest(error);
            }

            var events = await triggersManager.GetEventsTriggerDataAsync(triggersRequest, UserId, cancellationToken);

            var response = new DataResponse
            {
                Data = events,
            };

            return Ok(response);
        }
    }
}