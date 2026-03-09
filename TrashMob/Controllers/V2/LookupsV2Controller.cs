namespace TrashMob.Controllers.V2
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for lookup/reference data endpoints.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/lookups")]
    public class LookupsV2Controller(
        ILookupManager<EventType> eventTypeManager,
        ILookupManager<ServiceType> serviceTypeManager,
        ILookupManager<EventPartnerLocationServiceStatus> partnerServiceStatusManager,
        ILogger<LookupsV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets all event types.
        /// </summary>
        /// <returns>A list of event types.</returns>
        /// <response code="200">Returns the event type list.</response>
        [HttpGet("event-types")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventTypes()
        {
            logger.LogInformation("V2 GetEventTypes requested");

            var types = await eventTypeManager.GetAsync();
            var dtos = types.Select(t => t.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets all service types.
        /// </summary>
        /// <returns>A list of service types.</returns>
        /// <response code="200">Returns the service type list.</response>
        [HttpGet("service-types")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServiceTypes()
        {
            logger.LogInformation("V2 GetServiceTypes requested");

            var types = await serviceTypeManager.GetAsync();
            var dtos = types.Select(t => t.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets all event partner location service statuses.
        /// </summary>
        /// <returns>A list of partner service statuses.</returns>
        /// <response code="200">Returns the status list.</response>
        [HttpGet("partner-service-statuses")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPartnerServiceStatuses()
        {
            logger.LogInformation("V2 GetPartnerServiceStatuses requested");

            var statuses = await partnerServiceStatusManager.GetAsync();
            var dtos = statuses.Select(s => s.ToV2Dto()).ToList();

            return Ok(dtos);
        }
    }
}
