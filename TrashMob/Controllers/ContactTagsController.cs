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
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for contact tag management (admin only).
    /// </summary>
    [Route("api/contacttags")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class ContactTagsController(IKeyedManager<ContactTag> contactTagManager)
        : SecureController
    {
        /// <summary>
        /// Gets all contact tags.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContactTag>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var tags = await contactTagManager.GetAsync(cancellationToken);
            return Ok(tags);
        }

        /// <summary>
        /// Creates a new contact tag.
        /// </summary>
        /// <param name="contactTag">The tag to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [ProducesResponseType(typeof(ContactTag), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(ContactTag contactTag, CancellationToken cancellationToken)
        {
            var result = await contactTagManager.AddAsync(contactTag, UserId, cancellationToken);
            TrackEvent("AddContactTag");
            return Ok(result);
        }

        /// <summary>
        /// Updates a contact tag.
        /// </summary>
        /// <param name="contactTag">The updated tag data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut]
        [ProducesResponseType(typeof(ContactTag), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(ContactTag contactTag, CancellationToken cancellationToken)
        {
            var result = await contactTagManager.UpdateAsync(contactTag, UserId, cancellationToken);
            TrackEvent("UpdateContactTag");
            return Ok(result);
        }

        /// <summary>
        /// Deletes a contact tag.
        /// </summary>
        /// <param name="id">The tag ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await contactTagManager.DeleteAsync(id, cancellationToken);
            TrackEvent("DeleteContactTag");
            return NoContent();
        }
    }
}
