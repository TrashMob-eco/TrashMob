namespace TrashMobMobile.Data;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using TrashMob.Models;
using TrashMobMobile.Models;

public class PickupLocationRestService : RestServiceBase, IPickupLocationRestService
{
    protected override string Controller => "pickuplocations";

    public PickupLocationRestService()
    {
    }

    public async Task<PickupLocation> GetPickupLocationAsync(Guid pickupLocationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestUri = Controller + "/" + pickupLocationId;

            using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<PickupLocation>(content);                    
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }

    public async Task<PickupLocation> UpdatePickupLocationAsync(PickupLocation pickupLocation, CancellationToken cancellationToken = default)
    {
        try
        {
            var content = JsonContent.Create(pickupLocation, typeof(PickupLocation), null, SerializerOptions);

            using (var response = await AuthorizedHttpClient.PutAsync(Controller, content, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<PickupLocation>(responseContent);
                return result;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }

    public async Task<PickupLocation> AddPickupLocationAsync(PickupLocation pickupLocation, CancellationToken cancellationToken = default)
    {
        try
        {
            var content = JsonContent.Create(pickupLocation, typeof(PickupLocation), null, SerializerOptions);

            using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<PickupLocation>(responseContent);
                return result;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<PickupLocation>> DeletePickupLocationAsync(PickupLocation pickupLocation, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestUri = Controller + "/" + pickupLocation.Id;

            using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
            }

            return await GetPickupLocationsAsync(pickupLocation.EventId, cancellationToken);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<PickupLocation>> GetPickupLocationsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestUri = Controller + "/getbyevent/" + eventId;

            using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<List<PickupLocation>>(content);
                return result;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }

    public async Task SubmitLocationsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestUri = Controller + "/submit/" + eventId;

            using (var response = await AuthorizedHttpClient.PostAsync(requestUri, null, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }

    public async Task<string> GetPickupLocationImageAsync(Guid pickupLocationId, ImageSizeEnum imageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestUri = Controller + "/image/" + pickupLocationId + "/" + imageSize;

            using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                return content.TrimStart('"').TrimEnd('"');
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }

    public async Task AddPickupLocationImageAsync(Guid eventId, Guid pickupLocationId, string localFileName, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestUri = Controller + "/image/" + eventId;

            using (var stream = File.OpenRead(localFileName))
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

                var streamContent = new StreamContent(stream);
                streamContent.Headers.Add("Content-Type", "image/jpeg");

                var content = new MultipartFormDataContent
                {
                    { streamContent, "formFile", Path.GetFileName(localFileName)},
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
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }
}
