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
    /// Controller for contact note management (admin only).
    /// </summary>
    [Route("api/contactnotes")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class ContactNotesController(IContactNoteManager contactNoteManager)
        : SecureController
    {
        /// <summary>
        /// Gets all notes for a specific contact.
        /// </summary>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("bycontact/{contactId}")]
        [ProducesResponseType(typeof(IEnumerable<ContactNote>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByContactId(Guid contactId, CancellationToken cancellationToken)
        {
            var notes = await contactNoteManager.GetByContactIdAsync(contactId, cancellationToken);
            return Ok(notes);
        }

        /// <summary>
        /// Creates a new contact note.
        /// </summary>
        /// <param name="contactNote">The note to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [ProducesResponseType(typeof(ContactNote), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(ContactNote contactNote, CancellationToken cancellationToken)
        {
            var result = await contactNoteManager.AddAsync(contactNote, UserId, cancellationToken);
            TrackEvent("AddContactNote");
            return CreatedAtAction(nameof(GetByContactId), new { contactId = result.ContactId }, result);
        }

        /// <summary>
        /// Updates a contact note.
        /// </summary>
        /// <param name="contactNote">The updated note data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut]
        [ProducesResponseType(typeof(ContactNote), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(ContactNote contactNote, CancellationToken cancellationToken)
        {
            var result = await contactNoteManager.UpdateAsync(contactNote, UserId, cancellationToken);
            TrackEvent("UpdateContactNote");
            return Ok(result);
        }

        /// <summary>
        /// Deletes a contact note.
        /// </summary>
        /// <param name="id">The note ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await contactNoteManager.DeleteAsync(id, cancellationToken);
            TrackEvent("DeleteContactNote");
            return NoContent();
        }
    }
}
