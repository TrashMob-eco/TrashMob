namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using System.Text.Json;
using TrashMobMobile.Models;

public class NewsletterPreferenceRestService(IHttpClientFactory httpClientFactory)
    : RestServiceBase(httpClientFactory), INewsletterPreferenceRestService
{
    protected override string Controller => "newsletter-preferences";

    public async Task<IEnumerable<NewsletterCategoryDto>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/categories";

        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonSerializer.Deserialize<List<NewsletterCategoryDto>>(content, SerializerOptions) ?? [];
    }

    public async Task<IEnumerable<NewsletterPreferenceDto>> GetMyPreferencesAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await AuthorizedHttpClient.GetAsync(Controller, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonSerializer.Deserialize<List<NewsletterPreferenceDto>>(content, SerializerOptions) ?? [];
    }

    public async Task UpdatePreferenceAsync(int categoryId, bool isSubscribed,
        CancellationToken cancellationToken = default)
    {
        var body = new { categoryId, isSubscribed };

        using var response = await AuthorizedHttpClient.PutAsJsonAsync(Controller, body, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task UnsubscribeAllAsync(CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/unsubscribe-all";

        using var response = await AuthorizedHttpClient.PostAsync(requestUri, null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
