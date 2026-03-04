namespace TrashMobMobile.Services
{
    using System.Net;
    using System.Net.Http.Json;
    using Newtonsoft.Json;
    using TrashMob.Models;

    public class DependentRestService(IHttpClientFactory httpClientFactory)
        : RestServiceBase(httpClientFactory), IDependentRestService
    {
        protected override string Controller => "users/";

        // Dependent CRUD

        public async Task<List<Dependent>> GetDependentsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var requestUri = userId + "/dependents";

            using var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<List<Dependent>>(content) ?? [];
        }

        public async Task<Dependent> AddDependentAsync(Guid userId, Dependent dependent, CancellationToken cancellationToken = default)
        {
            var requestUri = userId + "/dependents";
            var body = JsonContent.Create(dependent, typeof(Dependent), null, SerializerOptions);

            using var response = await AuthorizedHttpClient.PostAsync(requestUri, body, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<Dependent>(content)!;
        }

        public async Task<Dependent> UpdateDependentAsync(Guid userId, Dependent dependent, CancellationToken cancellationToken = default)
        {
            var requestUri = userId + "/dependents/" + dependent.Id;
            var body = JsonContent.Create(dependent, typeof(Dependent), null, SerializerOptions);

            using var response = await AuthorizedHttpClient.PutAsync(requestUri, body, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<Dependent>(content)!;
        }

        public async Task DeleteDependentAsync(Guid userId, Guid dependentId, CancellationToken cancellationToken = default)
        {
            var requestUri = userId + "/dependents/" + dependentId;

            using var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        // Dependent Waivers

        public async Task<DependentWaiver> SignWaiverAsync(
            Guid dependentId, Guid waiverVersionId, string typedLegalName, CancellationToken cancellationToken = default)
        {
            using var client = CreateHttpClient("dependents/");
            var requestUri = dependentId + "/waiver";
            var body = JsonContent.Create(
                new { WaiverVersionId = waiverVersionId, TypedLegalName = typedLegalName },
                options: SerializerOptions);

            using var response = await client.PostAsync(requestUri, body, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<DependentWaiver>(content)!;
        }

        public async Task<DependentWaiver?> GetCurrentWaiverAsync(Guid dependentId, CancellationToken cancellationToken = default)
        {
            using var client = CreateHttpClient("dependents/");
            var requestUri = dependentId + "/waiver";

            using var response = await client.GetAsync(requestUri, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<DependentWaiver>(content);
        }

        // Event Dependents

        public async Task<List<EventDependent>> RegisterDependentsForEventAsync(
            Guid eventId, List<Guid> dependentIds, CancellationToken cancellationToken = default)
        {
            using var client = CreateHttpClient("events/");
            var requestUri = eventId + "/dependents";
            var body = JsonContent.Create(
                new { DependentIds = dependentIds },
                options: SerializerOptions);

            using var response = await client.PostAsync(requestUri, body, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<List<EventDependent>>(content) ?? [];
        }

        public async Task<List<EventDependent>> GetEventDependentsAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            using var client = CreateHttpClient("events/");
            var requestUri = eventId + "/dependents";

            using var response = await client.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<List<EventDependent>>(content) ?? [];
        }

        public async Task<int> GetEventDependentCountAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            using var client = CreateHttpClient("events/");
            var requestUri = eventId + "/dependents/count";

            using var response = await client.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<int>(content);
        }

        public async Task UnregisterDependentFromEventAsync(Guid eventId, Guid dependentId, CancellationToken cancellationToken = default)
        {
            using var client = CreateHttpClient("events/");
            var requestUri = eventId + "/dependents/" + dependentId;

            using var response = await client.DeleteAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
