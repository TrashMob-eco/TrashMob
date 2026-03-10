namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using Newtonsoft.Json;
using TrashMob.Models;
using TrashMob.Models.Extensions.V2;
using TrashMob.Models.Poco.V2;
using TrashMobMobile.Models;

public class PickupLocationRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IPickupLocationRestService
{
    protected override string Controller => "pickuplocations";

    public async Task<PickupLocation> GetPickupLocationAsync(Guid pickupLocationId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/" + pickupLocationId;

        using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var dto = JsonConvert.DeserializeObject<PickupLocationDto>(content)!;
            return dto.ToEntity();
        }
    }

    public async Task<PickupLocation> UpdatePickupLocationAsync(PickupLocation pickupLocation,
        CancellationToken cancellationToken = default)
    {
        var dto = pickupLocation.ToV2Dto();
        var content = JsonContent.Create(dto, typeof(PickupLocationDto), null, SerializerOptions);
        var requestUri = Controller + "/" + pickupLocation.Id;

        using (var response = await AuthorizedHttpClient.PutAsync(requestUri, content, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var resultDto = JsonConvert.DeserializeObject<PickupLocationDto>(responseContent)!;
            return resultDto.ToEntity();
        }
    }

    public async Task<PickupLocation> AddPickupLocationAsync(PickupLocation pickupLocation,
        CancellationToken cancellationToken = default)
    {
        var dto = pickupLocation.ToV2Dto();
        var content = JsonContent.Create(dto, typeof(PickupLocationDto), null, SerializerOptions);

        using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var resultDto = JsonConvert.DeserializeObject<PickupLocationDto>(responseContent)!;
            return resultDto.ToEntity();
        }
    }

    public async Task<IEnumerable<PickupLocation>> DeletePickupLocationAsync(PickupLocation pickupLocation,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/" + pickupLocation.Id;

        using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
        }

        return await GetPickupLocationsAsync(pickupLocation.EventId, cancellationToken);
    }

    public async Task<IEnumerable<PickupLocation>> GetPickupLocationsAsync(Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/by-event/" + eventId;

        using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var dtos = JsonConvert.DeserializeObject<List<PickupLocationDto>>(content) ?? [];
            return dtos.Select(d => d.ToEntity()).ToList();
        }
    }

    public async Task SubmitLocationsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/submit/" + eventId;

        using (var response = await AuthorizedHttpClient.PostAsync(requestUri, null, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
        }
    }

    public async Task<string> GetPickupLocationImageAsync(Guid pickupLocationId, ImageSizeEnum imageSize,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/" + pickupLocationId + "/image";

        using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return content.TrimStart('"').TrimEnd('"');
        }
    }

    public async Task AddPickupLocationImageAsync(Guid eventId, Guid pickupLocationId, string localFileName,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/" + pickupLocationId + "/image";

        using (var stream = File.OpenRead(localFileName))
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            var streamContent = new StreamContent(stream);
            streamContent.Headers.Add("Content-Type", "image/jpeg");

            var content = new MultipartFormDataContent
                {
                    { streamContent, "formFile", Path.GetFileName(localFileName) },
                    { new StringContent(pickupLocationId.ToString()), "parentId" },
                    { new StringContent(ImageUploadType.Pickup), "imageType" },
                };

            request.Content = content;

            using (var response = await AuthorizedHttpClient.SendAsync(request, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
