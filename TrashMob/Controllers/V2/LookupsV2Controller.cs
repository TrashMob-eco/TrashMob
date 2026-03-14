namespace TrashMob.Controllers.V2
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
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
        ILookupManager<EventStatus> eventStatusManager,
        ILookupManager<ServiceType> serviceTypeManager,
        ILookupManager<EventPartnerLocationServiceStatus> partnerServiceStatusManager,
        ILookupManager<PartnerType> partnerTypeManager,
        ILookupManager<PartnerStatus> partnerStatusManager,
        ILookupManager<PartnerRequestStatus> partnerRequestStatusManager,
        ILookupManager<WeightUnit> weightUnitManager,
        ILookupManager<InvitationStatus> invitationStatusManager,
        ILookupManager<SocialMediaAccountType> socialMediaAccountTypeManager,
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
        public async Task<IActionResult> GetEventTypes(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetEventTypes requested");

            var types = await eventTypeManager.GetAsync();
            var dtos = types.Select(t => t.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets all event statuses.
        /// </summary>
        /// <returns>A list of event statuses.</returns>
        /// <response code="200">Returns the event status list.</response>
        [HttpGet("event-statuses")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventStatuses(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetEventStatuses requested");

            var statuses = await eventStatusManager.GetAsync();
            var dtos = statuses.Select(s => s.ToV2Dto()).ToList();

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
        public async Task<IActionResult> GetServiceTypes(CancellationToken cancellationToken = default)
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
        public async Task<IActionResult> GetPartnerServiceStatuses(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetPartnerServiceStatuses requested");

            var statuses = await partnerServiceStatusManager.GetAsync();
            var dtos = statuses.Select(s => s.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets all partner types.
        /// </summary>
        /// <returns>A list of partner types.</returns>
        /// <response code="200">Returns the partner type list.</response>
        [HttpGet("partner-types")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPartnerTypes(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetPartnerTypes requested");

            var types = await partnerTypeManager.GetAsync();
            var dtos = types.Select(t => t.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets all partner statuses.
        /// </summary>
        /// <returns>A list of partner statuses.</returns>
        /// <response code="200">Returns the partner status list.</response>
        [HttpGet("partner-statuses")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPartnerStatuses(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetPartnerStatuses requested");

            var statuses = await partnerStatusManager.GetAsync();
            var dtos = statuses.Select(s => s.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets all partner request statuses.
        /// </summary>
        /// <returns>A list of partner request statuses.</returns>
        /// <response code="200">Returns the partner request status list.</response>
        [HttpGet("partner-request-statuses")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPartnerRequestStatuses(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetPartnerRequestStatuses requested");

            var statuses = await partnerRequestStatusManager.GetAsync();
            var dtos = statuses.Select(s => s.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets all weight units.
        /// </summary>
        /// <returns>A list of weight units.</returns>
        /// <response code="200">Returns the weight unit list.</response>
        [HttpGet("weight-units")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWeightUnits(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetWeightUnits requested");

            var units = await weightUnitManager.GetAsync();
            var dtos = units.Select(u => u.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets all invitation statuses.
        /// </summary>
        /// <returns>A list of invitation statuses.</returns>
        /// <response code="200">Returns the invitation status list.</response>
        [HttpGet("invitation-statuses")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInvitationStatuses(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetInvitationStatuses requested");

            var statuses = await invitationStatusManager.GetAsync();
            var dtos = statuses.Select(s => s.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets all social media account types.
        /// </summary>
        /// <returns>A list of social media account types.</returns>
        /// <response code="200">Returns the social media account type list.</response>
        [HttpGet("social-media-account-types")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(IReadOnlyList<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSocialMediaAccountTypes(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetSocialMediaAccountTypes requested");

            var types = await socialMediaAccountTypeManager.GetAsync();
            var dtos = types.Select(t => t.ToV2Dto()).ToList();

            return Ok(dtos);
        }
    }
}
