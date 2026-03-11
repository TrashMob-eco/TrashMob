namespace TrashMobMobile.Services;

using System.Linq;
using System.Net.Http.Json;
using TrashMob.Models;
using TrashMob.Models.Extensions.V2;
using TrashMob.Models.Poco.V2;
using TrashMobMobile.Models;

public class WaiverRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IWaiverRestService
{
    protected override string Controller => "waivers";

    public async Task<List<WaiverVersion>> GetRequiredWaiversAsync(Guid? communityId = null, CancellationToken cancellationToken = default)
    {
        var requestUri = communityId.HasValue
            ? $"{Controller}/required?communityId={communityId.Value}"
            : $"{Controller}/required";

        var dtos = await AuthorizedHttpClient.GetFromJsonAsync<List<WaiverVersionDto>>(requestUri, SerializerOptions, cancellationToken);
        return dtos?.Select(d => d.ToEntity()).ToList() ?? [];
    }

    public async Task<List<UserWaiver>> GetMyWaiversAsync(CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/my";
        var dtos = await AuthorizedHttpClient.GetFromJsonAsync<List<UserWaiverDto>>(requestUri, SerializerOptions, cancellationToken);
        return dtos?.Select(d => d.ToEntity()).ToList() ?? [];
    }

    public async Task<UserWaiver> AcceptWaiverAsync(AcceptWaiverApiRequest request, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/accept";
        var dto = new AcceptWaiverRequestDto
        {
            WaiverVersionId = request.WaiverVersionId,
            TypedLegalName = request.TypedLegalName,
            IsMinor = request.IsMinor,
            GuardianUserId = request.GuardianUserId,
            GuardianName = request.GuardianName ?? string.Empty,
            GuardianRelationship = request.GuardianRelationship ?? string.Empty,
        };
        var content = JsonContent.Create(dto, typeof(AcceptWaiverRequestDto), null, SerializerOptions);

        using var response = await AuthorizedHttpClient.PostAsync(requestUri, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var resultDto = await response.Content.ReadFromJsonAsync<UserWaiverDto>(SerializerOptions, cancellationToken);
        return resultDto!.ToEntity();
    }
}
