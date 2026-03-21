#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using TrashMob.Models.Poco.V2;

namespace TrashMob.Shared.Services
{
    public interface IWeatherService
    {
        /// <summary>
        /// Gets a weather forecast for a specific location and date/time window.
        /// </summary>
        /// <param name="latitude">Location latitude.</param>
        /// <param name="longitude">Location longitude.</param>
        /// <param name="eventDate">Event start date/time (UTC).</param>
        /// <param name="durationHours">Event duration in hours.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Weather forecast DTO. IsAvailable will be false if date is beyond forecast range.</returns>
        Task<WeatherForecastDto> GetForecastAsync(
            double latitude,
            double longitude,
            DateTimeOffset eventDate,
            int durationHours,
            CancellationToken cancellationToken = default);
    }
}
