namespace TrashMobMobile.Services;

using TrashMobMobile.Models;

public class NewsletterPreferenceManager(INewsletterPreferenceRestService restService)
    : INewsletterPreferenceManager
{
    public Task<IEnumerable<NewsletterCategoryDto>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        return restService.GetCategoriesAsync(cancellationToken);
    }

    public Task<IEnumerable<NewsletterPreferenceDto>> GetMyPreferencesAsync(
        CancellationToken cancellationToken = default)
    {
        return restService.GetMyPreferencesAsync(cancellationToken);
    }

    public Task UpdatePreferenceAsync(int categoryId, bool isSubscribed,
        CancellationToken cancellationToken = default)
    {
        return restService.UpdatePreferenceAsync(categoryId, isSubscribed, cancellationToken);
    }

    public Task UnsubscribeAllAsync(CancellationToken cancellationToken = default)
    {
        return restService.UnsubscribeAllAsync(cancellationToken);
    }
}
