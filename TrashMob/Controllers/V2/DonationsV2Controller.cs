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
    /// V2 controller for donation management (admin only).
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/donations")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class DonationsV2Controller(
        IDonationManager donationManager,
        IDonationEmailManager donationEmailManager,
        ILogger<DonationsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all donations.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of donations.</returns>
        /// <response code="200">Returns the donation list.</response>
        [HttpGet]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<DonationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetAllDonations");

            var donations = await donationManager.GetAsync(cancellationToken);
            var dtos = donations.Select(d => d.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets a donation by ID.
        /// </summary>
        /// <param name="id">The donation ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The donation.</returns>
        /// <response code="200">Returns the donation.</response>
        /// <response code="404">Donation not found.</response>
        [HttpGet("{id}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(DonationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var donation = await donationManager.GetAsync(id, cancellationToken);

            if (donation is null)
            {
                return NotFound();
            }

            return Ok(donation.ToV2Dto());
        }

        /// <summary>
        /// Gets all donations for a specific contact.
        /// </summary>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of donations for the contact.</returns>
        /// <response code="200">Returns the donation list.</response>
        [HttpGet("bycontact/{contactId}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<DonationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByContactId(Guid contactId, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetDonationsByContact: contactId={ContactId}", contactId);

            var donations = await donationManager.GetByContactIdAsync(contactId, cancellationToken);
            var dtos = donations.Select(d => d.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Creates a new donation.
        /// </summary>
        /// <param name="dto">The donation data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created donation.</returns>
        /// <response code="201">Donation created successfully.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(DonationDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] DonationDto dto, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 CreateDonation: contactId={ContactId}", dto.ContactId);

            var entity = dto.ToEntity();
            var result = await donationManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates a donation.
        /// </summary>
        /// <param name="dto">The updated donation data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated donation.</returns>
        /// <response code="200">Donation updated successfully.</response>
        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(DonationDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] DonationDto dto, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 UpdateDonation: id={Id}", dto.Id);

            var entity = dto.ToEntity();
            entity.LastUpdatedByUserId = UserId;
            var result = await donationManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a donation.
        /// </summary>
        /// <param name="id">The donation ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Donation deleted successfully.</response>
        [HttpDelete("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 DeleteDonation: id={Id}", id);

            await donationManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Sends a thank-you email for a donation.
        /// </summary>
        /// <param name="id">The donation ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Thank-you email sent successfully.</response>
        /// <response code="404">Donation not found.</response>
        [HttpPost("{id}/send-thankyou")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendThankYou(Guid id, CancellationToken cancellationToken = default)
        {
            var donation = await donationManager.GetAsync(id, cancellationToken);

            if (donation is null)
            {
                return NotFound();
            }

            logger.LogInformation("V2 SendDonationThankYou: id={Id}", id);

            await donationEmailManager.SendThankYouAsync(id, UserId, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Sends a tax receipt email with PDF attachment for a donation.
        /// </summary>
        /// <param name="id">The donation ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Receipt email sent successfully.</response>
        /// <response code="404">Donation not found.</response>
        [HttpPost("{id}/send-receipt")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendReceipt(Guid id, CancellationToken cancellationToken = default)
        {
            var donation = await donationManager.GetAsync(id, cancellationToken);

            if (donation is null)
            {
                return NotFound();
            }

            logger.LogInformation("V2 SendDonationReceipt: id={Id}", id);

            await donationEmailManager.SendReceiptAsync(id, UserId, cancellationToken);

            return NoContent();
        }
    }
}
