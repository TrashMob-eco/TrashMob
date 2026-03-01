namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Contacts;

    /// <summary>
    /// Controller for CRM contact management (admin only).
    /// </summary>
    [Route("api/contacts")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class ContactsController(IContactManager contactManager)
        : SecureController
    {
        /// <summary>
        /// Gets all contacts, optionally filtered by search term, contact type, or tag.
        /// </summary>
        /// <param name="search">Optional search term.</param>
        /// <param name="contactType">Optional contact type filter.</param>
        /// <param name="tagId">Optional tag filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Contact>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string search,
            [FromQuery] int? contactType,
            [FromQuery] Guid? tagId,
            CancellationToken cancellationToken)
        {
            IEnumerable<Contact> contacts;

            if (!string.IsNullOrWhiteSpace(search))
            {
                contacts = await contactManager.SearchAsync(search, cancellationToken);
            }
            else if (tagId.HasValue)
            {
                contacts = await contactManager.GetByTagAsync(tagId.Value, cancellationToken);
            }
            else if (contactType.HasValue)
            {
                contacts = await contactManager.GetByContactTypeAsync(contactType.Value, cancellationToken);
            }
            else
            {
                contacts = await contactManager.GetAsync(cancellationToken);
            }

            return Ok(contacts);
        }

        /// <summary>
        /// Gets a contact by ID.
        /// </summary>
        /// <param name="id">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var contact = await contactManager.GetAsync(id, cancellationToken);

            if (contact is null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        /// <summary>
        /// Creates a new contact.
        /// </summary>
        /// <param name="contact">The contact to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Contact contact, CancellationToken cancellationToken)
        {
            var result = await contactManager.AddAsync(contact, UserId, cancellationToken);
            TrackEvent("AddContact");
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates a contact.
        /// </summary>
        /// <param name="contact">The updated contact data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Contact contact, CancellationToken cancellationToken)
        {
            var result = await contactManager.UpdateAsync(contact, UserId, cancellationToken);
            TrackEvent("UpdateContact");
            return Ok(result);
        }

        /// <summary>
        /// Deletes a contact.
        /// </summary>
        /// <param name="id">The contact ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await contactManager.DeleteAsync(id, cancellationToken);
            TrackEvent("DeleteContact");
            return NoContent();
        }
    }
}
