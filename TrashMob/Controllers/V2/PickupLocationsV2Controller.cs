namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for managing pickup locations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/pickuplocations")]
    public class PickupLocationsV2Controller(
        IPickupLocationManager pickupLocationManager,
        IEventManager eventManager,
        IImageManager imageManager,
        IAuthorizationService authorizationService,
        ILogger<PickupLocationsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets a pickup location by ID.
        /// </summary>
        /// <param name="id">The pickup location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the pickup location.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PickupLocationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPickupLocation Id={Id}", id);

            var entity = await pickupLocationManager.GetAsync(id, cancellationToken);
            if (entity is null)
            {
                return NotFound();
            }

            return Ok(entity.ToV2Dto());
        }

        /// <summary>
        /// Gets pickup locations by event ID.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the pickup locations for the event.</response>
        [HttpGet("by-event/{eventId}")]
        [ProducesResponseType(typeof(IReadOnlyList<PickupLocationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByEvent(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPickupLocationsByEvent Event={EventId}", eventId);

            var entities = await pickupLocationManager.GetByParentIdAsync(eventId, cancellationToken);
            var dtos = entities.Select(e => e.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Adds a new pickup location.
        /// </summary>
        /// <param name="dto">The pickup location data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Pickup location created.</response>
        /// <response code="403">Not authorized.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PickupLocationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Add(PickupLocationDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddPickupLocation for Event={EventId}", dto.EventId);

            var mobEvent = await eventManager.GetAsync(dto.EventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            var result = await pickupLocationManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates a pickup location.
        /// </summary>
        /// <param name="id">The pickup location ID.</param>
        /// <param name="dto">The updated pickup location data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated pickup location.</response>
        /// <response code="403">Not authorized.</response>
        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PickupLocationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, PickupLocationDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdatePickupLocation Id={Id}", id);

            var localPickupLocation = await pickupLocationManager.GetAsync(id, cancellationToken);
            if (localPickupLocation is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(localPickupLocation, AuthorizationPolicyConstants.UserIsEventLead))
            {
                var mobEvent = await eventManager.GetAsync(localPickupLocation.EventId, cancellationToken);
                if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
                {
                    return Forbid();
                }
            }

            localPickupLocation.County = dto.County;
            localPickupLocation.Latitude = dto.Latitude;
            localPickupLocation.Longitude = dto.Longitude;
            localPickupLocation.Name = dto.Name;
            localPickupLocation.Notes = dto.Notes;
            localPickupLocation.PostalCode = dto.PostalCode;
            localPickupLocation.Region = dto.Region;
            localPickupLocation.StreetAddress = dto.StreetAddress;
            localPickupLocation.Country = dto.Country;
            localPickupLocation.City = dto.City;
            localPickupLocation.HasBeenPickedUp = dto.HasBeenPickedUp;
            localPickupLocation.HasBeenSubmitted = dto.HasBeenSubmitted;

            var result = await pickupLocationManager.UpdateAsync(localPickupLocation, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a pickup location.
        /// </summary>
        /// <param name="id">The pickup location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Pickup location deleted.</response>
        /// <response code="403">Not authorized.</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeletePickupLocation Id={Id}", id);

            var entity = await pickupLocationManager.GetAsync(id, cancellationToken);
            if (entity is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(entity, AuthorizationPolicyConstants.UserIsEventLead))
            {
                var mobEvent = await eventManager.GetAsync(entity.EventId, cancellationToken);
                if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
                {
                    return Forbid();
                }
            }

            await pickupLocationManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Submits pickup locations for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Pickup locations submitted.</response>
        /// <response code="403">Not authorized.</response>
        [HttpPost("submit/{eventId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Submit(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 SubmitPickupLocations for Event={EventId}", eventId);

            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            await pickupLocationManager.SubmitPickupLocationsAsync(eventId, UserId, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Uploads an image for a pickup location.
        /// </summary>
        /// <param name="id">The pickup location ID.</param>
        /// <param name="imageUpload">The image upload data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Image uploaded.</response>
        /// <response code="403">Not authorized.</response>
        [HttpPost("{id}/image")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadImage(Guid id, [FromForm] ImageUpload imageUpload, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UploadPickupLocationImage Id={Id}", id);

            var entity = await pickupLocationManager.GetAsync(id, cancellationToken);
            if (entity is null)
            {
                return NotFound();
            }

            var mobEvent = await eventManager.GetAsync(entity.EventId, cancellationToken);
            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            await imageManager.UploadImageAsync(imageUpload);

            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Gets the image URL for a pickup location.
        /// </summary>
        /// <param name="id">The pickup location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the image URL.</response>
        /// <response code="204">No image found.</response>
        [HttpGet("{id}/image")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetImage(Guid id, CancellationToken cancellationToken)
        {
            var url = await imageManager.GetImageUrlAsync(id, ImageTypeEnum.Pickup, ImageSizeEnum.Raw, cancellationToken);

            if (string.IsNullOrWhiteSpace(url))
            {
                return NoContent();
            }

            return Ok(url);
        }

        private async Task<bool> IsAuthorizedAsync(object resource, string policy)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var authResult = await authorizationService.AuthorizeAsync(User, resource, policy);
            return authResult.Succeeded;
        }
    }
}
