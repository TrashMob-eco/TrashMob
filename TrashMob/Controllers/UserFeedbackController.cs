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
    /// Controller for managing user feedback submissions.
    /// </summary>
    [Route("api/feedback")]
    public class UserFeedbackController : SecureController
    {
        private readonly IUserFeedbackManager feedbackManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFeedbackController"/> class.
        /// </summary>
        /// <param name="feedbackManager">The feedback manager.</param>
        public UserFeedbackController(IUserFeedbackManager feedbackManager)
        {
            this.feedbackManager = feedbackManager;
        }

        /// <summary>
        /// Submits new user feedback. Can be called by authenticated or anonymous users.
        /// </summary>
        /// <param name="feedback">The feedback to submit.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created feedback with ID.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserFeedback), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitFeedback([FromBody] UserFeedback feedback, CancellationToken cancellationToken)
        {
            if (feedback == null)
            {
                return BadRequest("Feedback cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(feedback.Category))
            {
                return BadRequest("Category is required.");
            }

            if (string.IsNullOrWhiteSpace(feedback.Description))
            {
                return BadRequest("Description is required.");
            }

            // Validate category
            var validCategories = new[] { "Bug", "FeatureRequest", "General", "Praise" };
            if (!Array.Exists(validCategories, c => c.Equals(feedback.Category, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Invalid category. Must be one of: Bug, FeatureRequest, General, Praise");
            }

            // Try to get UserId if authenticated
            try
            {
                if (HttpContext.Items.ContainsKey("UserId") && HttpContext.Items["UserId"] != null)
                {
                    feedback.UserId = new Guid(HttpContext.Items["UserId"].ToString());
                }
            }
            catch
            {
                // Anonymous user - UserId remains null
            }

            var result = await feedbackManager.AddAsync(feedback, cancellationToken);

            TrackEvent("SubmitUserFeedback");

            return CreatedAtAction(nameof(GetFeedback), new { id = result.Id }, result);
        }

        /// <summary>
        /// Gets a specific feedback item by ID. Admin only.
        /// </summary>
        /// <param name="id">The feedback ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The feedback item.</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(UserFeedback), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFeedback(Guid id, CancellationToken cancellationToken)
        {
            var feedback = await feedbackManager.GetAsync(id, cancellationToken);

            if (feedback == null)
            {
                return NotFound();
            }

            return Ok(feedback);
        }

        /// <summary>
        /// Gets all feedback, optionally filtered by status. Admin only.
        /// </summary>
        /// <param name="status">Optional status filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of feedback items.</returns>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<UserFeedback>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllFeedback([FromQuery] string status, CancellationToken cancellationToken)
        {
            var feedback = await feedbackManager.GetByStatusAsync(status, cancellationToken);

            TrackEvent("GetAllUserFeedback");

            return Ok(feedback);
        }

        /// <summary>
        /// Updates the status of a feedback item. Admin only.
        /// </summary>
        /// <param name="id">The feedback ID.</param>
        /// <param name="request">The update request containing status and notes.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated feedback item.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(UserFeedback), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateFeedback(Guid id, [FromBody] UpdateFeedbackRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return BadRequest("Request body is required.");
            }

            // Validate status
            var validStatuses = new[] { "New", "Reviewed", "Resolved", "Deferred" };
            if (!string.IsNullOrWhiteSpace(request.Status) && !Array.Exists(validStatuses, s => s.Equals(request.Status, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Invalid status. Must be one of: New, Reviewed, Resolved, Deferred");
            }

            var feedback = await feedbackManager.UpdateStatusAsync(id, request.Status, request.InternalNotes, UserId, cancellationToken);

            if (feedback == null)
            {
                return NotFound();
            }

            TrackEvent("UpdateUserFeedback");

            return Ok(feedback);
        }

        /// <summary>
        /// Deletes a feedback item. Admin only.
        /// </summary>
        /// <param name="id">The feedback ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFeedback(Guid id, CancellationToken cancellationToken)
        {
            var existing = await feedbackManager.GetAsync(id, cancellationToken);
            if (existing == null)
            {
                return NotFound();
            }

            await feedbackManager.DeleteAsync(id, cancellationToken);

            TrackEvent("DeleteUserFeedback");

            return NoContent();
        }
    }

    /// <summary>
    /// Request model for updating feedback status.
    /// </summary>
    public class UpdateFeedbackRequest
    {
        /// <summary>
        /// Gets or sets the new status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets internal notes.
        /// </summary>
        public string InternalNotes { get; set; }
    }
}
