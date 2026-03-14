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
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Contacts;

    /// <summary>
    /// V2 controller for CRM contact note management (admin only).
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/contactnotes")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class ContactNotesV2Controller(
        IContactNoteManager contactNoteManager,
        ILogger<ContactNotesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all notes for a specific contact.
        /// </summary>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of contact notes.</returns>
        /// <response code="200">Returns the note list.</response>
        [HttpGet("bycontact/{contactId}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<ContactNoteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByContactId(Guid contactId, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetContactNotesByContact: contactId={ContactId}", contactId);

            var notes = await contactNoteManager.GetByContactIdAsync(contactId, cancellationToken);
            var dtos = notes.Select(n => n.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Creates a new contact note.
        /// </summary>
        /// <param name="dto">The contact note data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created contact note.</returns>
        /// <response code="201">Note created successfully.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ContactNoteDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] ContactNoteDto dto, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 CreateContactNote: contactId={ContactId}", dto.ContactId);

            var entity = dto.ToEntity();
            var result = await contactNoteManager.AddAsync(entity, UserId, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, result.ToV2Dto());
        }

        /// <summary>
        /// Updates a contact note.
        /// </summary>
        /// <param name="dto">The updated contact note data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated contact note.</returns>
        /// <response code="200">Note updated successfully.</response>
        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ContactNoteDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] ContactNoteDto dto, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 UpdateContactNote: id={Id}", dto.Id);

            var entity = dto.ToEntity();
            entity.LastUpdatedByUserId = UserId;
            var result = await contactNoteManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a contact note.
        /// </summary>
        /// <param name="id">The note ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Note deleted successfully.</response>
        [HttpDelete("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 DeleteContactNote: id={Id}", id);

            await contactNoteManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
