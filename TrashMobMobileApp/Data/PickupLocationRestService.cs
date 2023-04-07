namespace TrashMobMobileApp.Data
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Config;
    using TrashMobMobileApp.Models;

    public class PickupLocationRestService : RestServiceBase, IPickupLocationRestService
    {
        protected override string Controller => "pickuplocations";

        public PickupLocationRestService(IOptions<Settings> settings)
            : base(settings)
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
                    var result = JsonConvert.DeserializeObject<List<PickupLocation>>(content);
                    return result.FirstOrDefault();
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
                }

                return await GetPickupLocationAsync(pickupLocation.Id, cancellationToken);
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
                }

                return await GetPickupLocationAsync(pickupLocation.Id, cancellationToken);
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

        public async Task AddPickupLocationImageAsync(Guid eventId, Guid pickupLocationId, string localFileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = Controller + "/image/" + eventId;

                using (var stream = File.OpenRead(localFileName))
                {
                    var pickupImage = new ImageUpload()
                    {
                        ParentId = pickupLocationId,
                        ImageType = ImageTypeEnum.Pickup,
                        FormFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                    };

                    var content = JsonContent.Create(pickupImage, typeof(ImageUpload), null, SerializerOptions);

                    using (var response = await AuthorizedHttpClient.PostAsync(requestUri, content, cancellationToken))
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
}
