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
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for user feedback submissions and admin management.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/feedback")]
    public class UserFeedbackV2Controller(
        IUserFeedbackManager feedbackManager,
        ILogger<UserFeedbackV2Controller> logger) : ControllerBase
    {
        private static readonly string[] ValidCategories = ["Bug", "FeatureRequest", "General", "Praise"];
        private static readonly string[] ValidStatuses = ["New", "Reviewed", "Resolved", "Deferred"];

        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Submits new user feedback. Can be called by authenticated or anonymous users.
        /// </summary>
        /// <param name="dto">The feedback to submit.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created feedback.</returns>
        /// <response code="201">Feedback created successfully.</response>
        /// <response code="400">Invalid request data.</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserFeedbackDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitFeedback([FromBody] UserFeedbackWriteDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
            {
                return Problem("Feedback cannot be null.", statusCode: StatusCodes.Status400BadRequest);
            }

            if (string.IsNullOrWhiteSpace(dto.Category))
            {
                return Problem("Category is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            if (string.IsNullOrWhiteSpace(dto.Description))
            {
                return Problem("Description is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            if (!Array.Exists(ValidCategories, c => c.Equals(dto.Category, StringComparison.OrdinalIgnoreCase)))
            {
                return Problem("Invalid category. Must be one of: Bug, FeatureRequest, General, Praise", statusCode: StatusCodes.Status400BadRequest);
            }

            var feedback = dto.ToEntity();

            // Try to get UserId if authenticated
            try
            {
                if (HttpContext.Items.ContainsKey("UserId") && HttpContext.Items["UserId"] is not null)
                {
                    feedback.UserId = new Guid(HttpContext.Items["UserId"].ToString()!);
                }
            }
            catch
            {
                // Anonymous user - UserId remains null
            }

            logger.LogInformation("V2 SubmitFeedback: category={Category}", dto.Category);

            var result = await feedbackManager.AddAsync(feedback, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, result.ToV2Dto());
        }

        /// <summary>
        /// Gets a specific feedback item by ID. Admin only.
        /// </summary>
        /// <param name="id">The feedback ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The feedback item.</returns>
        /// <response code="200">Returns the feedback item.</response>
        /// <response code="404">Feedback not found.</response>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(UserFeedbackDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFeedback(Guid id, CancellationToken cancellationToken = default)
        {
            var feedback = await feedbackManager.GetAsync(id, cancellationToken);

            if (feedback is null)
            {
                return NotFound();
            }

            return Ok(feedback.ToV2Dto());
        }

        /// <summary>
        /// Gets all feedback, optionally filtered by status. Admin only.
        /// </summary>
        /// <param name="status">Optional status filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of feedback items.</returns>
        /// <response code="200">Returns the feedback list.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<UserFeedbackDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllFeedback([FromQuery] string status = null, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetAllFeedback: status={Status}", status);

            var feedback = await feedbackManager.GetByStatusAsync(status, cancellationToken);
            var dtos = feedback.Select(f => f.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Updates the status of a feedback item. Admin only.
        /// </summary>
        /// <param name="id">The feedback ID.</param>
        /// <param name="dto">The update request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated feedback item.</returns>
        /// <response code="200">Feedback updated successfully.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="404">Feedback not found.</response>
        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(UserFeedbackDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFeedback(Guid id, [FromBody] UpdateFeedbackStatusDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
            {
                return Problem("Request body is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            if (!string.IsNullOrWhiteSpace(dto.Status) && !Array.Exists(ValidStatuses, s => s.Equals(dto.Status, StringComparison.OrdinalIgnoreCase)))
            {
                return Problem("Invalid status. Must be one of: New, Reviewed, Resolved, Deferred", statusCode: StatusCodes.Status400BadRequest);
            }

            logger.LogInformation("V2 UpdateFeedback: id={Id}, status={Status}", id, dto.Status);

            var feedback = await feedbackManager.UpdateStatusAsync(id, dto.Status, dto.InternalNotes, UserId, cancellationToken);

            if (feedback is null)
            {
                return NotFound();
            }

            return Ok(feedback.ToV2Dto());
        }

        /// <summary>
        /// Deletes a feedback item. Admin only.
        /// </summary>
        /// <param name="id">The feedback ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Feedback deleted successfully.</response>
        /// <response code="404">Feedback not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFeedback(Guid id, CancellationToken cancellationToken = default)
        {
            var existing = await feedbackManager.GetAsync(id, cancellationToken);
            if (existing is null)
            {
                return NotFound();
            }

            logger.LogInformation("V2 DeleteFeedback: id={Id}", id);

            await feedbackManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
