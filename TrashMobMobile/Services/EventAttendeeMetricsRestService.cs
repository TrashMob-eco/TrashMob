namespace TrashMobMobile.Services
{
    using System.Net;
    using System.Net.Http.Json;
    using Newtonsoft.Json;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public class EventAttendeeMetricsRestService(IHttpClientFactory httpClientFactory)
        : RestServiceBase(httpClientFactory), IEventAttendeeMetricsRestService
    {
        protected override string Controller => "events/";
        protected override bool UseV2 => true;

        public async Task<EventAttendeeMetrics?> GetMyMetricsAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var requestUri = eventId + "/attendee-metrics/my-metrics";

            using var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<EventAttendeeMetrics>(content);
        }

        public async Task<EventAttendeeMetrics> SubmitMyMetricsAsync(Guid eventId, EventAttendeeMetrics metrics, CancellationToken cancellationToken = default)
        {
            var requestUri = eventId + "/attendee-metrics/my-metrics";
            var body = JsonContent.Create(metrics, typeof(EventAttendeeMetrics), null, SerializerOptions);

            using var response = await AuthorizedHttpClient.PostAsync(requestUri, body, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<EventAttendeeMetrics>(content)!;
        }

        public async Task<EventMetricsPublicSummary> GetPublicMetricsAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var requestUri = eventId + "/attendee-metrics/public";

            using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<EventMetricsPublicSummary>(content) ?? new EventMetricsPublicSummary();
        }

        public async Task<List<EventAttendeeMetrics>> GetPendingMetricsAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var requestUri = eventId + "/attendee-metrics/pending";

            using var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<List<EventAttendeeMetrics>>(content) ?? [];
        }

        public async Task<int> ApproveAllPendingAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var requestUri = eventId + "/attendee-metrics/approve-all";

            using var response = await AuthorizedHttpClient.PostAsync(requestUri, null, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<int>(content);
        }

        public async Task<EventAttendeeMetrics> ApproveMetricsAsync(Guid eventId, Guid metricsId, CancellationToken cancellationToken = default)
        {
            var requestUri = eventId + "/attendee-metrics/" + metricsId + "/approve";

            using var response = await AuthorizedHttpClient.PostAsync(requestUri, null, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<EventAttendeeMetrics>(content)!;
        }

        public async Task<EventAttendeeMetrics> RejectMetricsAsync(Guid eventId, Guid metricsId, string reason, CancellationToken cancellationToken = default)
        {
            var requestUri = eventId + "/attendee-metrics/" + metricsId + "/reject";
            var body = JsonContent.Create(new { RejectionReason = reason }, null, SerializerOptions);

            using var response = await AuthorizedHttpClient.PostAsync(requestUri, body, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<EventAttendeeMetrics>(content)!;
        }

        public async Task<UserImpactStats> GetUserImpactAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            // This endpoint is on the users controller: api/users/{userId}/impact
            // Need to use a different base path
            using var httpClient = CreateHttpClient("users/");

            var requestUri = userId + "/impact";

            using var response = await httpClient.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<UserImpactStats>(content) ?? new UserImpactStats();
        }
    }
}
