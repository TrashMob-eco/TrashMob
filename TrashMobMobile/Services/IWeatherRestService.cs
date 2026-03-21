namespace TrashMobMobile.Services;

using TrashMob.Models.Poco.V2;

public interface IWeatherRestService
{
    Task<WeatherForecastDto> GetForecastAsync(double latitude, double longitude, DateTimeOffset eventDate,
        int durationHours, CancellationToken cancellationToken = default);
}
