namespace TrashMobMobile.Services;

using TrashMobMobile.Models;

public interface INewsletterPreferenceRestService
{
    Task<IEnumerable<NewsletterCategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<NewsletterPreferenceDto>> GetMyPreferencesAsync(CancellationToken cancellationToken = default);

    Task UpdatePreferenceAsync(int categoryId, bool isSubscribed, CancellationToken cancellationToken = default);

    Task UnsubscribeAllAsync(CancellationToken cancellationToken = default);
}
