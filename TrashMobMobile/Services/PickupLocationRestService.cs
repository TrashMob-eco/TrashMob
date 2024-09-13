namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using Newtonsoft.Json;
using TrashMob.Models;
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
            return JsonConvert.DeserializeObject<PickupLocation>(content);
        }
    }

    public async Task<PickupLocation> UpdatePickupLocationAsync(PickupLocation pickupLocation,
        CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(pickupLocation, typeof(PickupLocation), null, SerializerOptions);

        using (var response = await AuthorizedHttpClient.PutAsync(Controller, content, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<PickupLocation>(responseContent);
            return result;
        }
    }

    public async Task<PickupLocation> AddPickupLocationAsync(PickupLocation pickupLocation,
        CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(pickupLocation, typeof(PickupLocation), null, SerializerOptions);

        using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<PickupLocation>(responseContent);
            return result;
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
        var requestUri = Controller + "/getbyevent/" + eventId;

        using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<List<PickupLocation>>(content);
            return result;
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
        var requestUri = Controller + "/image/" + pickupLocationId + "/" + imageSize;

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
        var requestUri = Controller + "/image/" + eventId;

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