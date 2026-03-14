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

    /// <summary>
    /// V2 controller for CRM contact tag management (admin only).
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/contacttags")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class ContactTagsV2Controller(
        IKeyedManager<ContactTag> contactTagManager,
        ILogger<ContactTagsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all contact tags.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of contact tags.</returns>
        /// <response code="200">Returns the tag list.</response>
        [HttpGet]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<ContactTagDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetAllContactTags");

            var tags = await contactTagManager.GetAsync(cancellationToken);
            var dtos = tags.Select(t => t.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Creates a new contact tag.
        /// </summary>
        /// <param name="dto">The contact tag data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created contact tag.</returns>
        /// <response code="201">Tag created successfully.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ContactTagDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] ContactTagDto dto, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 CreateContactTag: name={Name}", dto.Name);

            var entity = dto.ToEntity();
            var result = await contactTagManager.AddAsync(entity, UserId, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, result.ToV2Dto());
        }

        /// <summary>
        /// Updates a contact tag.
        /// </summary>
        /// <param name="dto">The updated contact tag data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated contact tag.</returns>
        /// <response code="200">Tag updated successfully.</response>
        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ContactTagDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] ContactTagDto dto, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 UpdateContactTag: id={Id}", dto.Id);

            var entity = dto.ToEntity();
            entity.LastUpdatedByUserId = UserId;
            var result = await contactTagManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a contact tag.
        /// </summary>
        /// <param name="id">The tag ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Tag deleted successfully.</response>
        [HttpDelete("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 DeleteContactTag: id={Id}", id);

            await contactTagManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
