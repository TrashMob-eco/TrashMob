namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using System.Text.Json;
using TrashMob.Models;

public class EventPhotoRestService(IHttpClientFactory httpClientFactory)
    : RestServiceBase(httpClientFactory), IEventPhotoRestService
{
    protected override string Controller => "events";

    public async Task<IEnumerable<EventPhoto>> GetEventPhotosAsync(Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/" + eventId + "/photos";

        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrEmpty(content))
        {
            return [];
        }

        return JsonSerializer.Deserialize<IEnumerable<EventPhoto>>(content, SerializerOptions) ?? [];
    }

    public async Task<EventPhoto> UploadPhotoAsync(Guid eventId, string localFilePath,
        EventPhotoType photoType, string caption, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/" + eventId + "/photos";
        var photoId = Guid.NewGuid();

        using var stream = File.OpenRead(localFilePath);
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

        var streamContent = new StreamContent(stream);
        streamContent.Headers.Add("Content-Type", "image/jpeg");

        var formContent = new MultipartFormDataContent
        {
            { streamContent, "formFile", Path.GetFileName(localFilePath) },
            { new StringContent(photoId.ToString()), "parentId" },
            { new StringContent("EventPhoto"), "imageType" },
        };

        request.Content = formContent;

        using var response = await AuthorizedHttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonSerializer.Deserialize<EventPhoto>(responseContent, SerializerOptions)
               ?? throw new InvalidOperationException("Failed to deserialize uploaded photo response.");
    }

    public async Task DeletePhotoAsync(Guid eventId, Guid photoId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/" + eventId + "/photos/" + photoId;

        using var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
