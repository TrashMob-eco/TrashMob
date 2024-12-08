namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using TrashMob.Models;
using TrashMob.Models.Poco;

public class EventLitterReportRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IEventLitterReportRestService
{
    protected override string Controller => "eventlitterreports";

    public async Task<IEnumerable<FullEventLitterReport>> GetEventLitterReportsAsync(Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = string.Concat(Controller, $"/{eventId}");
        var eventLitterReports =
            await AuthorizedHttpClient.GetFromJsonAsync<IEnumerable<FullEventLitterReport>>(requestUri, SerializerOptions,
                cancellationToken);
        return eventLitterReports ?? [];
    }

    public async Task<FullEventLitterReport> GetEventLitterReportByLitterReportIdAsync(Guid litterReportId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = string.Concat(Controller, $"/GetByLitterReportId/{litterReportId}");
        var eventLitterReport =
            await AuthorizedHttpClient.GetFromJsonAsync<FullEventLitterReport>(requestUri, SerializerOptions,
                cancellationToken);
        return eventLitterReport;
    }

    public async Task AddLitterReportAsync(EventLitterReport eventLitterReport, CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(eventLitterReport, typeof(EventLitterReport), null, SerializerOptions);
        var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveLitterReportAsync(EventLitterReport eventLitterReport, CancellationToken cancellationToken = default)
    {
        var requestUri = string.Concat(Controller, $"/{eventLitterReport.EventId}/{eventLitterReport.LitterReportId}");

        using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
        }
    }
}