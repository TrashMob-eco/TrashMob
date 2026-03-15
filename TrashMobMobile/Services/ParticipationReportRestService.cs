namespace TrashMobMobile.Services
{
    using System.Net.Http.Json;
    using Newtonsoft.Json;

    public class ParticipationReportRestService(IHttpClientFactory httpClientFactory)
        : RestServiceBase(httpClientFactory), IParticipationReportRestService
    {
        protected override string Controller => "events/";

        public async Task RequestReportAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var requestUri = eventId + "/participation-report";

            using var response = await AuthorizedHttpClient.PostAsync(requestUri, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<int> SendAllReportsAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var requestUri = eventId + "/participation-report/send-all";

            using var response = await AuthorizedHttpClient.PostAsync(requestUri, null, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeAnonymousType(content, new { sentCount = 0 });
            return result?.sentCount ?? 0;
        }
    }
}
