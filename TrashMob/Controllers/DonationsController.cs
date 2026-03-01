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
    /// Controller for donation management (admin only).
    /// </summary>
    [Route("api/donations")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class DonationsController(IDonationManager donationManager)
        : SecureController
    {
        /// <summary>
        /// Gets all donations.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Donation>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var donations = await donationManager.GetAsync(cancellationToken);
            return Ok(donations);
        }

        /// <summary>
        /// Gets a donation by ID.
        /// </summary>
        /// <param name="id">The donation ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Donation), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var donation = await donationManager.GetAsync(id, cancellationToken);

            if (donation is null)
            {
                return NotFound();
            }

            return Ok(donation);
        }

        /// <summary>
        /// Gets all donations for a specific contact.
        /// </summary>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("bycontact/{contactId}")]
        [ProducesResponseType(typeof(IEnumerable<Donation>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByContactId(Guid contactId, CancellationToken cancellationToken)
        {
            var donations = await donationManager.GetByContactIdAsync(contactId, cancellationToken);
            return Ok(donations);
        }

        /// <summary>
        /// Creates a new donation.
        /// </summary>
        /// <param name="donation">The donation to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [ProducesResponseType(typeof(Donation), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Donation donation, CancellationToken cancellationToken)
        {
            var result = await donationManager.AddAsync(donation, UserId, cancellationToken);
            TrackEvent("AddDonation");
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates a donation.
        /// </summary>
        /// <param name="donation">The updated donation data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut]
        [ProducesResponseType(typeof(Donation), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Donation donation, CancellationToken cancellationToken)
        {
            var result = await donationManager.UpdateAsync(donation, UserId, cancellationToken);
            TrackEvent("UpdateDonation");
            return Ok(result);
        }

        /// <summary>
        /// Deletes a donation.
        /// </summary>
        /// <param name="id">The donation ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await donationManager.DeleteAsync(id, cancellationToken);
            TrackEvent("DeleteDonation");
            return NoContent();
        }
    }
}
