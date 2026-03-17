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
    using TrashMob.Shared.Managers.Contacts;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for CRM contact management (admin only).
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/contacts")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class ContactsV2Controller(
        IContactManager contactManager,
        IBaseManager<ContactContactTag> contactContactTagManager,
        ILogger<ContactsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all contacts, optionally filtered by search term, contact type, or tag.
        /// </summary>
        /// <param name="search">Optional search term.</param>
        /// <param name="contactType">Optional contact type filter (-1 means no filter).</param>
        /// <param name="tagId">Optional tag filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of contacts.</returns>
        /// <response code="200">Returns the contact list.</response>
        [HttpGet]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<ContactDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string search = null,
            [FromQuery] int contactType = -1,
            [FromQuery] Guid tagId = default,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetAllContacts: search={Search}, contactType={ContactType}, tagId={TagId}", search, contactType, tagId);

            IEnumerable<Contact> contacts;

            if (!string.IsNullOrWhiteSpace(search))
            {
                contacts = await contactManager.SearchAsync(search, cancellationToken);
            }
            else if (tagId != default)
            {
                contacts = await contactManager.GetByTagAsync(tagId, cancellationToken);
            }
            else if (contactType >= 0)
            {
                contacts = await contactManager.GetByContactTypeAsync(contactType, cancellationToken);
            }
            else
            {
                contacts = await contactManager.GetAsync(cancellationToken);
            }

            var dtos = contacts.Select(c => c.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets a contact by ID.
        /// </summary>
        /// <param name="id">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The contact.</returns>
        /// <response code="200">Returns the contact.</response>
        /// <response code="404">Contact not found.</response>
        [HttpGet("{id}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var contact = await contactManager.GetAsync(id, cancellationToken);

            if (contact is null)
            {
                return NotFound();
            }

            return Ok(contact.ToV2Dto());
        }

        /// <summary>
        /// Creates a new contact.
        /// </summary>
        /// <param name="dto">The contact data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created contact.</returns>
        /// <response code="201">Contact created successfully.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] ContactDto dto, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 CreateContact");

            var entity = dto.ToEntity();
            var result = await contactManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates a contact.
        /// </summary>
        /// <param name="dto">The updated contact data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated contact.</returns>
        /// <response code="200">Contact updated successfully.</response>
        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] ContactDto dto, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 UpdateContact: id={Id}", dto.Id);

            var entity = dto.ToEntity();
            entity.LastUpdatedByUserId = UserId;
            var result = await contactManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a contact.
        /// </summary>
        /// <param name="id">The contact ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Contact deleted successfully.</response>
        [HttpDelete("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 DeleteContact: id={Id}", id);

            await contactManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Replaces all tag assignments for a contact.
        /// </summary>
        /// <param name="id">The contact ID.</param>
        /// <param name="tagIds">The tag IDs to assign.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Tags updated successfully.</response>
        [HttpPut("{id}/tags")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateTags(Guid id, [FromBody] Guid[] tagIds, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 UpdateContactTags: contactId={Id}, tagCount={Count}", id, tagIds.Length);

            var existing = await contactContactTagManager.GetAsync(ct => ct.ContactId == id, cancellationToken);

            foreach (var tag in existing)
            {
                await contactContactTagManager.Delete(tag.ContactId, tag.ContactTagId, cancellationToken);
            }

            foreach (var tagId in tagIds)
            {
                var newTag = new ContactContactTag { ContactId = id, ContactTagId = tagId };
                await contactContactTagManager.AddAsync(newTag, UserId, cancellationToken);
            }

            return NoContent();
        }

        /// <summary>
        /// Gets the tag IDs assigned to a contact.
        /// </summary>
        /// <param name="id">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of tag IDs.</returns>
        /// <response code="200">Returns the tag ID list.</response>
        [HttpGet("{id}/tags")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTags(Guid id, CancellationToken cancellationToken = default)
        {
            var tags = await contactContactTagManager.GetAsync(ct => ct.ContactId == id, cancellationToken);
            var tagIds = tags.Select(ct => ct.ContactTagId).ToList();

            return Ok(tagIds);
        }
    }
}
