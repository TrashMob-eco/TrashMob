namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Controller for managing newsletters (admin only).
    /// </summary>
    [Authorize]
    [Route("api/admin/newsletters")]
    public class NewslettersController : SecureController
    {
        private readonly INewsletterManager newsletterManager;
        private readonly ILookupRepository<NewsletterCategory> categoryRepository;
        private readonly ILookupRepository<NewsletterTemplate> templateRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewslettersController"/> class.
        /// </summary>
        /// <param name="newsletterManager">The newsletter manager.</param>
        /// <param name="categoryRepository">The category repository.</param>
        /// <param name="templateRepository">The template repository.</param>
        public NewslettersController(
            INewsletterManager newsletterManager,
            ILookupRepository<NewsletterCategory> categoryRepository,
            ILookupRepository<NewsletterTemplate> templateRepository)
        {
            this.newsletterManager = newsletterManager;
            this.categoryRepository = categoryRepository;
            this.templateRepository = templateRepository;
        }

        /// <summary>
        /// Gets all newsletters with optional status filter.
        /// </summary>
        /// <param name="status">Optional status filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of newsletters.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetNewsletters([FromQuery] string status = null, CancellationToken cancellationToken = default)
        {
            if (!await IsAuthorizedAsync(null, AuthorizationPolicyConstants.UserIsAdmin))
            {
                return Forbid();
            }

            var newsletters = await newsletterManager.GetNewslettersAsync(status, cancellationToken);
            var result = newsletters.Select(n => new
            {
                n.Id,
                n.Subject,
                n.PreviewText,
                n.CategoryId,
                categoryName = n.Category?.Name,
                n.TargetType,
                n.TargetId,
                n.Status,
                n.ScheduledDate,
                n.SentDate,
                n.RecipientCount,
                n.SentCount,
                n.DeliveredCount,
                n.OpenCount,
                n.ClickCount,
                n.BounceCount,
                n.UnsubscribeCount,
                n.CreatedDate,
                n.LastUpdatedDate
            });

            return Ok(result);
        }

        /// <summary>
        /// Gets a newsletter by ID.
        /// </summary>
        /// <param name="id">The newsletter ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The newsletter.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNewsletter(Guid id, CancellationToken cancellationToken = default)
        {
            if (!await IsAuthorizedAsync(null, AuthorizationPolicyConstants.UserIsAdmin))
            {
                return Forbid();
            }

            var newsletter = await newsletterManager.GetAsync(id, cancellationToken);
            if (newsletter == null)
            {
                return NotFound();
            }

            return Ok(newsletter);
        }

        /// <summary>
        /// Creates a new newsletter draft.
        /// </summary>
        /// <param name="request">The create request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created newsletter.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateNewsletter([FromBody] CreateNewsletterRequest request, CancellationToken cancellationToken = default)
        {
            if (!await IsAuthorizedAsync(null, AuthorizationPolicyConstants.UserIsAdmin))
            {
                return Forbid();
            }

            if (string.IsNullOrEmpty(request.Subject))
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
                Status = "Draft"
            };

            var created = await newsletterManager.AddAsync(newsletter, UserId, cancellationToken);
            TrackEvent(nameof(CreateNewsletter));

            return CreatedAtAction(nameof(GetNewsletter), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing newsletter draft.
        /// </summary>
        /// <param name="id">The newsletter ID.</param>
        /// <param name="request">The update request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated newsletter.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateNewsletter(Guid id, [FromBody] UpdateNewsletterRequest request, CancellationToken cancellationToken = default)
        {
            if (!await IsAuthorizedAsync(null, AuthorizationPolicyConstants.UserIsAdmin))
            {
                return Forbid();
            }

            var newsletter = await newsletterManager.GetAsync(id, cancellationToken);
            if (newsletter == null)
            {
                return NotFound();
            }

            if (newsletter.Status != "Draft")
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

            await newsletterManager.UpdateAsync(newsletter, UserId, cancellationToken);
            TrackEvent(nameof(UpdateNewsletter));

            return Ok(newsletter);
        }

        /// <summary>
        /// Schedules a newsletter for sending.
        /// </summary>
        /// <param name="id">The newsletter ID.</param>
        /// <param name="request">The schedule request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The scheduled newsletter.</returns>
        [HttpPost("{id}/schedule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ScheduleNewsletter(Guid id, [FromBody] ScheduleNewsletterRequest request, CancellationToken cancellationToken = default)
        {
            if (!await IsAuthorizedAsync(null, AuthorizationPolicyConstants.UserIsAdmin))
            {
                return Forbid();
            }

            try
            {
                var newsletter = await newsletterManager.ScheduleNewsletterAsync(id, request.ScheduledDate, UserId, cancellationToken);
                TrackEvent(nameof(ScheduleNewsletter));
                return Ok(newsletter);
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
        /// <returns>The newsletter being sent.</returns>
        [HttpPost("{id}/send")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendNewsletter(Guid id, CancellationToken cancellationToken = default)
        {
            if (!await IsAuthorizedAsync(null, AuthorizationPolicyConstants.UserIsAdmin))
            {
                return Forbid();
            }

            try
            {
                var newsletter = await newsletterManager.SendNewsletterAsync(id, UserId, cancellationToken);
                TrackEvent(nameof(SendNewsletter));
                return Ok(new { message = "Newsletter queued for sending.", newsletter });
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
        /// <param name="request">The test send request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Success status.</returns>
        [HttpPost("{id}/test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendTestEmail(Guid id, [FromBody] TestSendRequest request, CancellationToken cancellationToken = default)
        {
            if (!await IsAuthorizedAsync(null, AuthorizationPolicyConstants.UserIsAdmin))
            {
                return Forbid();
            }

            if (request.Emails == null || !request.Emails.Any())
            {
                return BadRequest("At least one email address is required.");
            }

            try
            {
                await newsletterManager.SendTestEmailAsync(id, request.Emails, cancellationToken);
                TrackEvent(nameof(SendTestEmail));
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
        /// <returns>Success status.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNewsletter(Guid id, CancellationToken cancellationToken = default)
        {
            if (!await IsAuthorizedAsync(null, AuthorizationPolicyConstants.UserIsAdmin))
            {
                return Forbid();
            }

            var newsletter = await newsletterManager.GetAsync(id, cancellationToken);
            if (newsletter == null)
            {
                return NotFound();
            }

            if (newsletter.Status != "Draft")
            {
                return BadRequest("Only draft newsletters can be deleted.");
            }

            await newsletterManager.DeleteAsync(id, cancellationToken);
            TrackEvent(nameof(DeleteNewsletter));

            return Ok(new { message = "Newsletter deleted." });
        }

        /// <summary>
        /// Gets available newsletter templates.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of templates.</returns>
        [HttpGet("templates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetTemplates(CancellationToken cancellationToken = default)
        {
            if (!await IsAuthorizedAsync(null, AuthorizationPolicyConstants.UserIsAdmin))
            {
                return Forbid();
            }

            var templates = await templateRepository.Get(t => t.IsActive == true).ToListAsync(cancellationToken);
            var result = templates.OrderBy(t => t.DisplayOrder).Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                t.HtmlContent,
                t.TextContent,
                t.ThumbnailUrl
            });

            return Ok(result);
        }
    }

    /// <summary>
    /// Request model for creating a newsletter.
    /// </summary>
    public class CreateNewsletterRequest
    {
        /// <summary>
        /// Gets or sets the category ID.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the preview text.
        /// </summary>
        public string PreviewText { get; set; }

        /// <summary>
        /// Gets or sets the HTML content.
        /// </summary>
        public string HtmlContent { get; set; }

        /// <summary>
        /// Gets or sets the plain text content.
        /// </summary>
        public string TextContent { get; set; }

        /// <summary>
        /// Gets or sets the target type.
        /// </summary>
        public string TargetType { get; set; }

        /// <summary>
        /// Gets or sets the target ID.
        /// </summary>
        public Guid? TargetId { get; set; }
    }

    /// <summary>
    /// Request model for updating a newsletter.
    /// </summary>
    public class UpdateNewsletterRequest
    {
        /// <summary>
        /// Gets or sets the category ID.
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the preview text.
        /// </summary>
        public string PreviewText { get; set; }

        /// <summary>
        /// Gets or sets the HTML content.
        /// </summary>
        public string HtmlContent { get; set; }

        /// <summary>
        /// Gets or sets the plain text content.
        /// </summary>
        public string TextContent { get; set; }

        /// <summary>
        /// Gets or sets the target type.
        /// </summary>
        public string TargetType { get; set; }

        /// <summary>
        /// Gets or sets the target ID.
        /// </summary>
        public Guid? TargetId { get; set; }
    }

    /// <summary>
    /// Request model for scheduling a newsletter.
    /// </summary>
    public class ScheduleNewsletterRequest
    {
        /// <summary>
        /// Gets or sets the scheduled date.
        /// </summary>
        public DateTimeOffset ScheduledDate { get; set; }
    }

    /// <summary>
    /// Request model for sending a test email.
    /// </summary>
    public class TestSendRequest
    {
        /// <summary>
        /// Gets or sets the list of email addresses to send the test to.
        /// </summary>
        public List<string> Emails { get; set; }
    }
}
