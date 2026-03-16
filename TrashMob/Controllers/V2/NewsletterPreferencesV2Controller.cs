namespace TrashMob.Controllers.V2
{
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
    /// V2 controller for managing newsletter preferences.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/newsletter-preferences")]
    public class NewsletterPreferencesV2Controller(
        IUserNewsletterPreferenceManager preferenceManager,
        ILookupRepository<NewsletterCategory> categoryRepository,
        ILogger<NewsletterPreferencesV2Controller> logger) : ControllerBase
    {
        private System.Guid UserId => System.Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : System.Guid.Empty;

        /// <summary>
        /// Gets all newsletter categories.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the categories.</response>
        [HttpGet("categories")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<NewsletterCategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetNewsletterCategories");

            var categories = await categoryRepository.Get(c => c.IsActive == true)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync(cancellationToken);

            var dtos = categories.Select(c => c.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets the current user's newsletter preferences.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the user's preferences.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<NewsletterPreferenceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyPreferences(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetMyNewsletterPreferences");

            var preferences = await preferenceManager.GetUserPreferencesAsync(UserId, cancellationToken);

            var categories = await categoryRepository.Get(c => c.IsActive == true)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync(cancellationToken);

            var preferenceDict = preferences.ToDictionary(p => p.CategoryId);

            var result = categories.Select(c =>
            {
                var hasPreference = preferenceDict.TryGetValue(c.Id, out var pref);
                return new NewsletterPreferenceDto
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name ?? string.Empty,
                    CategoryDescription = c.Description ?? string.Empty,
                    IsSubscribed = hasPreference ? pref.IsSubscribed : c.IsDefault,
                    SubscribedDate = hasPreference ? pref.SubscribedDate : null,
                    UnsubscribedDate = hasPreference ? pref.UnsubscribedDate : null,
                };
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Updates a user's subscription preference for a category.
        /// </summary>
        /// <param name="request">The update request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated preference.</response>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(NewsletterPreferenceDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePreference(
            [FromBody] UpdateNewsletterPreferenceDto request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateNewsletterPreference Category={CategoryId}, IsSubscribed={IsSubscribed}",
                request.CategoryId, request.IsSubscribed);

            var preference = await preferenceManager.UpdatePreferenceAsync(
                UserId, request.CategoryId, request.IsSubscribed, cancellationToken);

            var category = await categoryRepository.Get(c => c.Id == preference.CategoryId)
                .FirstOrDefaultAsync(cancellationToken);

            var dto = new NewsletterPreferenceDto
            {
                CategoryId = preference.CategoryId,
                CategoryName = category?.Name ?? string.Empty,
                CategoryDescription = category?.Description ?? string.Empty,
                IsSubscribed = preference.IsSubscribed,
                SubscribedDate = preference.SubscribedDate,
                UnsubscribedDate = preference.UnsubscribedDate,
            };

            return Ok(dto);
        }

        /// <summary>
        /// Unsubscribes the user from all newsletter categories.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Unsubscribed from all.</response>
        [HttpPost("unsubscribe-all")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UnsubscribeAll(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UnsubscribeAllNewsletters");

            await preferenceManager.UnsubscribeAllAsync(UserId, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Processes an unsubscribe request using a token (no authentication required).
        /// Used for one-click unsubscribe from email links.
        /// </summary>
        /// <param name="request">The unsubscribe request containing the token.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Unsubscribe successful.</response>
        /// <response code="400">Token is missing or invalid.</response>
        [AllowAnonymous]
        [HttpPost("unsubscribe")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProcessUnsubscribe(
            [FromBody] UnsubscribeRequestDto request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ProcessUnsubscribe");

            if (string.IsNullOrWhiteSpace(request?.Token))
            {
                return Problem(detail: "Token is required.", statusCode: StatusCodes.Status400BadRequest, title: "Invalid request");
            }

            var result = await preferenceManager.ProcessUnsubscribeTokenAsync(request.Token, cancellationToken);

            if (!result.Success)
            {
                return Problem(detail: result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest, title: "Unsubscribe failed");
            }

            return Ok(new
            {
                success = true,
                email = result.Email,
                allCategories = result.AllCategories,
                categoryName = result.CategoryName,
                message = result.AllCategories
                    ? "You have been unsubscribed from all newsletters."
                    : $"You have been unsubscribed from {result.CategoryName} newsletters."
            });
        }
    }

    /// <summary>
    /// Request model for token-based unsubscribe in V2 API.
    /// </summary>
    public class UnsubscribeRequestDto
    {
        /// <summary>
        /// Gets or sets the unsubscribe token.
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}
