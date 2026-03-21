namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using TrashMob.Models.Poco.V2;

public class WeatherRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory),
    IWeatherRestService
{
    protected override string Controller => "weather";

    public async Task<WeatherForecastDto> GetForecastAsync(double latitude, double longitude,
        DateTimeOffset eventDate, int durationHours, CancellationToken cancellationToken = default)
    {
        var dateParam = Uri.EscapeDataString(eventDate.ToString("o"));
        var requestUri = $"forecast?lat={latitude}&lng={longitude}&date={dateParam}&durationHours={durationHours}";

        var httpClient = AnonymousHttpClient;
        var response = await httpClient.GetAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new WeatherForecastDto { IsAvailable = false };
        }

        var result = await response.Content.ReadFromJsonAsync<WeatherForecastDto>(SerializerOptions,
            cancellationToken);

        return result ?? new WeatherForecastDto { IsAvailable = false };
    }
}
