namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using TrashMob.Models;
using TrashMobMobile.Models;

public class WaiverRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IWaiverRestService
{
    protected override string Controller => "waivers";

    public async Task<List<WaiverVersion>> GetRequiredWaiversAsync(Guid? communityId = null, CancellationToken cancellationToken = default)
    {
        var requestUri = communityId.HasValue
            ? $"{Controller}/required?communityId={communityId.Value}"
            : $"{Controller}/required";

        var result = await AuthorizedHttpClient.GetFromJsonAsync<List<WaiverVersion>>(requestUri, SerializerOptions, cancellationToken);
        return result ?? [];
    }

    public async Task<List<UserWaiver>> GetMyWaiversAsync(CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/my";
        var result = await AuthorizedHttpClient.GetFromJsonAsync<List<UserWaiver>>(requestUri, SerializerOptions, cancellationToken);
        return result ?? [];
    }

    public async Task<UserWaiver> AcceptWaiverAsync(AcceptWaiverApiRequest request, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/accept";
        var content = JsonContent.Create(request, typeof(AcceptWaiverApiRequest), null, SerializerOptions);

        using var response = await AuthorizedHttpClient.PostAsync(requestUri, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<UserWaiver>(SerializerOptions, cancellationToken);
        return result!;
    }
}
