namespace TrashMob.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Controller for managing user newsletter preferences.
    /// </summary>
    [Authorize]
    [Route("api/newsletter-preferences")]
    public class NewsletterPreferencesController : SecureController
    {
        private readonly IUserNewsletterPreferenceManager preferenceManager;
        private readonly ILookupRepository<NewsletterCategory> categoryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsletterPreferencesController"/> class.
        /// </summary>
        /// <param name="preferenceManager">The preference manager.</param>
        /// <param name="categoryRepository">The category repository.</param>
        public NewsletterPreferencesController(
            IUserNewsletterPreferenceManager preferenceManager,
            ILookupRepository<NewsletterCategory> categoryRepository)
        {
            this.preferenceManager = preferenceManager;
            this.categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Gets all newsletter categories.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of newsletter categories.</returns>
        [AllowAnonymous]
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var categories = await categoryRepository.Get(c => c.IsActive == true)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync(cancellationToken);
            var result = categories.Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.IsDefault
            });
            return Ok(result);
        }

        /// <summary>
        /// Gets the current user's newsletter preferences.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of user preferences.</returns>
        [HttpGet]
        public async Task<IActionResult> GetMyPreferences(CancellationToken cancellationToken)
        {
            var preferences = await preferenceManager.GetUserPreferencesAsync(UserId, cancellationToken);

            // Get all categories to include ones user hasn't set yet
            var categories = await categoryRepository.Get(c => c.IsActive == true)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync(cancellationToken);
            var preferenceDict = preferences.ToDictionary(p => p.CategoryId);

            var result = categories.Select(c =>
            {
                var hasPreference = preferenceDict.TryGetValue(c.Id, out var pref);
                return new
                {
                    categoryId = c.Id,
                    categoryName = c.Name,
                    categoryDescription = c.Description,
                    isSubscribed = hasPreference ? pref.IsSubscribed : c.IsDefault,
                    subscribedDate = hasPreference ? pref.SubscribedDate : null,
                    unsubscribedDate = hasPreference ? pref.UnsubscribedDate : null
                };
            });

            return Ok(result);
        }

        /// <summary>
        /// Updates a user's subscription preference for a category.
        /// </summary>
        /// <param name="request">The update request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated preference.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdatePreference([FromBody] UpdatePreferenceRequest request, CancellationToken cancellationToken)
        {
            var preference = await preferenceManager.UpdatePreferenceAsync(
                UserId,
                request.CategoryId,
                request.IsSubscribed,
                cancellationToken);

            TrackEvent(request.IsSubscribed ? nameof(UpdatePreference) + "_Subscribe" : nameof(UpdatePreference) + "_Unsubscribe");

            return Ok(new
            {
                categoryId = preference.CategoryId,
                isSubscribed = preference.IsSubscribed,
                subscribedDate = preference.SubscribedDate,
                unsubscribedDate = preference.UnsubscribedDate
            });
        }

        /// <summary>
        /// Unsubscribes the user from all newsletter categories.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Success status.</returns>
        [HttpPost("unsubscribe-all")]
        public async Task<IActionResult> UnsubscribeAll(CancellationToken cancellationToken)
        {
            await preferenceManager.UnsubscribeAllAsync(UserId, cancellationToken);
            TrackEvent(nameof(UnsubscribeAll));
            return Ok(new { message = "Successfully unsubscribed from all newsletters." });
        }
    }

    /// <summary>
    /// Request model for updating newsletter preference.
    /// </summary>
    public class UpdatePreferenceRequest
    {
        /// <summary>
        /// Gets or sets the category ID.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets whether to subscribe or unsubscribe.
        /// </summary>
        public bool IsSubscribed { get; set; }
    }
}
