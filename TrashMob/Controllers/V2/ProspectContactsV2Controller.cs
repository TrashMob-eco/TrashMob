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
    using TrashMob.Shared.Managers.Prospects;

    /// <summary>
    /// V2 controller for managing the contacts associated with a CommunityProspect (Project 60).
    /// Admin-only — contacts are not exposed publicly.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/community-prospects/{prospectId}/contacts")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class ProspectContactsV2Controller(
        ICommunityProspectManager prospectManager,
        IProspectContactManager contactManager,
        ILogger<ProspectContactsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Lists all contacts for a prospect, primary first.
        /// </summary>
        /// <param name="prospectId">The parent prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the contacts list.</response>
        /// <response code="404">Prospect not found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProspectContactDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(Guid prospectId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetAll prospect contacts ProspectId={ProspectId}", prospectId);

            var prospect = await prospectManager.GetAsync(prospectId, cancellationToken);
            if (prospect is null)
            {
                return NotFound();
            }

            var contacts = await contactManager.GetByProspectAsync(prospectId, cancellationToken);
            return Ok(contacts.Select(c => c.ToV2Dto()).ToList());
        }

        /// <summary>
        /// Gets a single contact by ID.
        /// </summary>
        /// <param name="prospectId">The parent prospect ID.</param>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the contact.</response>
        /// <response code="404">Contact not found or doesn't belong to the given prospect.</response>
        [HttpGet("{contactId}")]
        [ProducesResponseType(typeof(ProspectContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid prospectId, Guid contactId, CancellationToken cancellationToken)
        {
            var contact = await contactManager.GetAsync(contactId, cancellationToken);
            if (contact is null || contact.ProspectId != prospectId)
            {
                return NotFound();
            }

            return Ok(contact.ToV2Dto());
        }

        /// <summary>
        /// Creates a new contact for a prospect. If <c>IsPrimary</c> is true on the incoming
        /// DTO, any existing primary contact on the prospect is demoted in the same operation.
        /// </summary>
        /// <param name="prospectId">The parent prospect ID.</param>
        /// <param name="dto">The contact data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Contact created.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="404">Prospect not found.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProspectContactDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(Guid prospectId, [FromBody] ProspectContactDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto?.Name))
            {
                return Problem("Contact name is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            var prospect = await prospectManager.GetAsync(prospectId, cancellationToken);
            if (prospect is null)
            {
                return NotFound();
            }

            logger.LogInformation("V2 Create prospect contact ProspectId={ProspectId} Name={Name}", prospectId, dto.Name);

            var entity = dto.ToEntity();
            entity.ProspectId = prospectId;

            var created = await contactManager.AddAsync(entity, UserId, cancellationToken);

            // If the caller asked for primary, run the atomic SetPrimaryAsync so existing
            // primaries get demoted.
            if (dto.IsPrimary)
            {
                created = await contactManager.SetPrimaryAsync(created.Id, UserId, cancellationToken);
            }

            return CreatedAtAction(nameof(Get), new { prospectId, contactId = created.Id }, created.ToV2Dto());
        }

        /// <summary>
        /// Updates a contact. The <c>IsPrimary</c> flag on the DTO is ignored here — use
        /// the dedicated <c>POST /{contactId}/set-primary</c> endpoint to change primary
        /// status atomically.
        /// </summary>
        /// <param name="prospectId">The parent prospect ID.</param>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="dto">The updated contact data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Contact updated.</response>
        /// <response code="404">Contact not found or doesn't belong to the given prospect.</response>
        [HttpPut("{contactId}")]
        [ProducesResponseType(typeof(ProspectContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid prospectId, Guid contactId, [FromBody] ProspectContactDto dto, CancellationToken cancellationToken)
        {
            var existing = await contactManager.GetAsync(contactId, cancellationToken);
            if (existing is null || existing.ProspectId != prospectId)
            {
                return NotFound();
            }

            logger.LogInformation("V2 Update prospect contact ContactId={ContactId}", contactId);

            existing.Name = dto.Name ?? existing.Name;
            existing.Title = dto.Title;
            existing.Email = dto.Email;
            existing.Phone = dto.Phone;
            existing.Role = dto.Role;
            existing.ContactStatus = dto.ContactStatus;
            existing.ReferredByContactId = dto.ReferredByContactId;
            existing.Notes = dto.Notes;
            existing.LastUpdatedByUserId = UserId;
            existing.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updated = await contactManager.UpdateAsync(existing, UserId, cancellationToken);
            return Ok(updated.ToV2Dto());
        }

        /// <summary>
        /// Deletes a contact. If the contact has any activity or outreach email references,
        /// the delete is blocked and a 409 is returned — the caller should mark the contact
        /// inactive (status = LeftOrganization) instead.
        /// </summary>
        /// <param name="prospectId">The parent prospect ID.</param>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Contact deleted.</response>
        /// <response code="404">Contact not found or doesn't belong to the given prospect.</response>
        /// <response code="409">Contact has history and cannot be deleted.</response>
        [HttpDelete("{contactId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(Guid prospectId, Guid contactId, CancellationToken cancellationToken)
        {
            var existing = await contactManager.GetAsync(contactId, cancellationToken);
            if (existing is null || existing.ProspectId != prospectId)
            {
                return NotFound();
            }

            if (await contactManager.HasReferencesAsync(contactId, cancellationToken))
            {
                return Problem(
                    detail: "Contact has activity or outreach history and cannot be deleted. Mark it as LeftOrganization instead.",
                    statusCode: StatusCodes.Status409Conflict);
            }

            logger.LogInformation("V2 Delete prospect contact ContactId={ContactId}", contactId);

            await contactManager.DeleteAsync(contactId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Atomically designates a contact as primary, clearing IsPrimary on all other
        /// contacts of the same prospect.
        /// </summary>
        /// <param name="prospectId">The parent prospect ID.</param>
        /// <param name="contactId">The contact ID to promote.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated contact.</response>
        /// <response code="404">Contact not found or doesn't belong to the given prospect.</response>
        [HttpPost("{contactId}/set-primary")]
        [ProducesResponseType(typeof(ProspectContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetPrimary(Guid prospectId, Guid contactId, CancellationToken cancellationToken)
        {
            var existing = await contactManager.GetAsync(contactId, cancellationToken);
            if (existing is null || existing.ProspectId != prospectId)
            {
                return NotFound();
            }

            logger.LogInformation("V2 SetPrimary prospect contact ContactId={ContactId}", contactId);

            var updated = await contactManager.SetPrimaryAsync(contactId, UserId, cancellationToken);
            return Ok(updated.ToV2Dto());
        }

        /// <summary>
        /// Updates the lifecycle status of a contact (0=Active, 1=WrongPerson, 2=NoResponse,
        /// 3=LeftOrganization, 4=RightPerson).
        /// </summary>
        /// <param name="prospectId">The parent prospect ID.</param>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="request">The new status.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated contact.</response>
        /// <response code="404">Contact not found or doesn't belong to the given prospect.</response>
        [HttpPut("{contactId}/status")]
        [ProducesResponseType(typeof(ProspectContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(
            Guid prospectId,
            Guid contactId,
            [FromBody] UpdateProspectContactStatusRequest request,
            CancellationToken cancellationToken)
        {
            var existing = await contactManager.GetAsync(contactId, cancellationToken);
            if (existing is null || existing.ProspectId != prospectId)
            {
                return NotFound();
            }

            logger.LogInformation(
                "V2 UpdateStatus prospect contact ContactId={ContactId} Status={Status}",
                contactId, request.Status);

            var updated = await contactManager.UpdateStatusAsync(contactId, request.Status, UserId, cancellationToken);
            return Ok(updated.ToV2Dto());
        }
    }

    /// <summary>
    /// Request body for changing a ProspectContact's lifecycle status (V2).
    /// </summary>
    public class UpdateProspectContactStatusRequest
    {
        /// <summary>
        /// Gets or sets the new <see cref="ProspectContactStatus"/> value.
        /// </summary>
        public int Status { get; set; }
    }
}
