namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Config;

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
                var content = JsonContent.Create(pickupLocation, typeof(EventSummary), null, SerializerOptions);

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
    }
}
