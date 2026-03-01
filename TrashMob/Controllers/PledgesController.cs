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
    /// Controller for pledge management (admin only).
    /// </summary>
    [Route("api/pledges")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class PledgesController(IPledgeManager pledgeManager)
        : SecureController
    {
        /// <summary>
        /// Gets all pledges.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Pledge>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var pledges = await pledgeManager.GetAsync(cancellationToken);
            return Ok(pledges);
        }

        /// <summary>
        /// Gets a pledge by ID.
        /// </summary>
        /// <param name="id">The pledge ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Pledge), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var pledge = await pledgeManager.GetAsync(id, cancellationToken);

            if (pledge is null)
            {
                return NotFound();
            }

            return Ok(pledge);
        }

        /// <summary>
        /// Gets all pledges for a specific contact.
        /// </summary>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("bycontact/{contactId}")]
        [ProducesResponseType(typeof(IEnumerable<Pledge>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByContactId(Guid contactId, CancellationToken cancellationToken)
        {
            var pledges = await pledgeManager.GetByContactIdAsync(contactId, cancellationToken);
            return Ok(pledges);
        }

        /// <summary>
        /// Creates a new pledge.
        /// </summary>
        /// <param name="pledge">The pledge to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [ProducesResponseType(typeof(Pledge), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Pledge pledge, CancellationToken cancellationToken)
        {
            var result = await pledgeManager.AddAsync(pledge, UserId, cancellationToken);
            TrackEvent("AddPledge");
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates a pledge.
        /// </summary>
        /// <param name="pledge">The updated pledge data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut]
        [ProducesResponseType(typeof(Pledge), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Pledge pledge, CancellationToken cancellationToken)
        {
            var result = await pledgeManager.UpdateAsync(pledge, UserId, cancellationToken);
            TrackEvent("UpdatePledge");
            return Ok(result);
        }

        /// <summary>
        /// Deletes a pledge.
        /// </summary>
        /// <param name="id">The pledge ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await pledgeManager.DeleteAsync(id, cancellationToken);
            TrackEvent("DeletePledge");
            return NoContent();
        }
    }
}
