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
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// V2 controller for managing newsletters (admin only).
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/admin/newsletters")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class NewslettersAdminV2Controller(
        INewsletterManager newsletterManager,
        ILookupRepository<NewsletterTemplate> templateRepository,
        ILogger<NewslettersAdminV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all newsletters with optional status filter.
        /// </summary>
        /// <param name="status">Optional status filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of newsletters.</returns>
        [HttpGet]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<NewsletterDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNewsletters([FromQuery] string status = null, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetNewsletters Status={Status}", status);

            var newsletters = await newsletterManager.GetNewslettersAsync(status, cancellationToken);

            return Ok(newsletters.Select(n => n.ToV2Dto()));
        }

        /// <summary>
        /// Gets a newsletter by ID.
        /// </summary>
        /// <param name="id">The newsletter ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The newsletter.</returns>
        [HttpGet("{id}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(NewsletterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNewsletter(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetNewsletter Id={Id}", id);

            var newsletter = await newsletterManager.GetAsync(id, cancellationToken);
            if (newsletter == null)
            {
                return NotFound();
            }

            return Ok(newsletter.ToV2Dto());
        }

        /// <summary>
        /// Creates a new newsletter draft.
        /// </summary>
        /// <param name="request">The create newsletter request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created newsletter.</returns>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(NewsletterDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateNewsletter([FromBody] CreateNewsletterDto request, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 CreateNewsletter Subject={Subject}", request?.Subject);

            if (string.IsNullOrWhiteSpace(request?.Subject))
            {
                return BadRequest("Subject is required.");
            }

            var newsletter = new Newsletter
            {
                CategoryId = request.CategoryId,
                Subject = request.Subject,
                PreviewText = request.PreviewText,
                HtmlContent = request.HtmlContent,
                TextContent = request.TextContent,
                TargetType = request.TargetType ?? "All",
                TargetId = request.TargetId,
                Status = NewsletterStatus.Draft
            };

            var created = await newsletterManager.AddAsync(newsletter, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetNewsletter), new { id = created.Id }, created.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing newsletter draft.
        /// </summary>
        /// <param name="id">The newsletter ID.</param>
        /// <param name="request">The update newsletter request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated newsletter.</returns>
        [HttpPut("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(NewsletterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateNewsletter(Guid id, [FromBody] UpdateNewsletterDto request, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 UpdateNewsletter Id={Id}", id);

            var newsletter = await newsletterManager.GetAsync(id, cancellationToken);
            if (newsletter == null)
            {
                return NotFound();
            }

            if (newsletter.Status != NewsletterStatus.Draft)
            {
                return BadRequest("Only draft newsletters can be updated.");
            }

            newsletter.CategoryId = request.CategoryId ?? newsletter.CategoryId;
            newsletter.Subject = request.Subject ?? newsletter.Subject;
            newsletter.PreviewText = request.PreviewText ?? newsletter.PreviewText;
            newsletter.HtmlContent = request.HtmlContent ?? newsletter.HtmlContent;
            newsletter.TextContent = request.TextContent ?? newsletter.TextContent;
            newsletter.TargetType = request.TargetType ?? newsletter.TargetType;
            newsletter.TargetId = request.TargetId ?? newsletter.TargetId;

            var updated = await newsletterManager.UpdateAsync(newsletter, UserId, cancellationToken);

            return Ok(updated.ToV2Dto());
        }

        /// <summary>
        /// Schedules a newsletter for sending at a specified date.
        /// </summary>
        /// <param name="id">The newsletter ID.</param>
        /// <param name="request">The schedule request containing the scheduled date.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The scheduled newsletter.</returns>
        [HttpPost("{id}/schedule")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(NewsletterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ScheduleNewsletter(Guid id, [FromBody] ScheduleNewsletterDto request, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 ScheduleNewsletter Id={Id} ScheduledDate={ScheduledDate}", id, request?.ScheduledDate);

            try
            {
                var newsletter = await newsletterManager.ScheduleNewsletterAsync(id, request.ScheduledDate, UserId, cancellationToken);

                return Ok(newsletter.ToV2Dto());
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Sends a newsletter immediately.
        /// </summary>
        /// <param name="id">The newsletter ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The newsletter being sent with a status message.</returns>
        [HttpPost("{id}/send")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendNewsletter(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 SendNewsletter Id={Id}", id);

            try
            {
                var newsletter = await newsletterManager.SendNewsletterAsync(id, UserId, cancellationToken);

                return Ok(new { message = "Newsletter queued for sending.", newsletter = newsletter.ToV2Dto() });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Sends a test email for a newsletter to specified addresses.
        /// </summary>
        /// <param name="id">The newsletter ID.</param>
        /// <param name="request">The test send request containing email addresses.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Success message.</returns>
        [HttpPost("{id}/test")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendTestEmail(Guid id, [FromBody] TestSendNewsletterDto request, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 SendTestEmail Id={Id}", id);

            if (request.Emails == null || !request.Emails.Any())
            {
                return BadRequest("At least one email address is required.");
            }

            try
            {
                await newsletterManager.SendTestEmailAsync(id, request.Emails, cancellationToken);

                return Ok(new { message = $"Test email sent to {request.Emails.Count} addresses." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a draft newsletter.
        /// </summary>
        /// <param name="id">The newsletter ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNewsletter(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 DeleteNewsletter Id={Id}", id);

            var newsletter = await newsletterManager.GetAsync(id, cancellationToken);
            if (newsletter == null)
            {
                return NotFound();
            }

            if (newsletter.Status != NewsletterStatus.Draft)
            {
                return BadRequest("Only draft newsletters can be deleted.");
            }

            await newsletterManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Gets available newsletter templates.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of active newsletter templates.</returns>
        [HttpGet("templates")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<NewsletterTemplateDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTemplates(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetTemplates");

            var templates = await templateRepository.Get(t => t.IsActive == true).ToListAsync(cancellationToken);

            return Ok(templates.OrderBy(t => t.DisplayOrder).Select(t => t.ToV2Dto()));
        }
    }
}
