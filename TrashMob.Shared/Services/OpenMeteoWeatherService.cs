#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TrashMob.Models.Poco.V2;

namespace TrashMob.Shared.Services
{
    public class OpenMeteoWeatherService(
        HttpClient httpClient,
        IMemoryCache memoryCache,
        ILogger<OpenMeteoWeatherService> logger) : IWeatherService
    {
        private const string BaseUrl = "https://api.open-meteo.com/v1/forecast";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public async Task<WeatherForecastDto> GetForecastAsync(
            double latitude,
            double longitude,
            DateTimeOffset eventDate,
            int durationHours,
            CancellationToken cancellationToken = default)
        {
            // Events in the past or beyond 16-day forecast range
            var daysUntilEvent = (eventDate.Date - DateTimeOffset.UtcNow.Date).Days;
            if (daysUntilEvent < 0 || daysUntilEvent > 16)
            {
                return new WeatherForecastDto { IsAvailable = false };
            }

            // Round coordinates to 2 decimal places for cache efficiency (~1.1km precision)
            var roundedLat = Math.Round(latitude, 2);
            var roundedLng = Math.Round(longitude, 2);
            var dateStr = eventDate.ToString("yyyy-MM-dd");
            var cacheKey = $"weather:{roundedLat:F2}:{roundedLng:F2}:{dateStr}";

            if (memoryCache.TryGetValue(cacheKey, out WeatherForecastDto? cached) && cached != null)
            {
                logger.LogDebug("Weather cache hit for {CacheKey}", cacheKey);
                return cached;
            }

            var forecast = await FetchFromApiAsync(roundedLat, roundedLng, dateStr, eventDate, durationHours, cancellationToken);

            memoryCache.Set(cacheKey, forecast, new MemoryCacheEntryOptions
            {
                SlidingExpiration = CacheDuration,
            });

            return forecast;
        }

        private async Task<WeatherForecastDto> FetchFromApiAsync(
            double latitude,
            double longitude,
            string dateStr,
            DateTimeOffset eventDate,
            int durationHours,
            CancellationToken cancellationToken)
        {
            var url = $"{BaseUrl}?latitude={latitude}&longitude={longitude}"
                + $"&hourly=temperature_2m,precipitation_probability,weather_code,wind_speed_10m"
                + $"&temperature_unit=fahrenheit&wind_speed_unit=mph"
                + $"&start_date={dateStr}&end_date={dateStr}&timezone=auto";

            logger.LogInformation("Fetching weather from Open-Meteo for Lat={Latitude}, Lng={Longitude}, Date={Date}",
                latitude, longitude, dateStr);

            try
            {
                var response = await httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Open-Meteo returned {StatusCode} for {Url}", response.StatusCode, url);
                    return new WeatherForecastDto { IsAvailable = false };
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var apiResponse = JsonSerializer.Deserialize<OpenMeteoResponse>(json, JsonOptions);

                if (apiResponse?.Hourly == null)
                {
                    logger.LogWarning("Open-Meteo response missing hourly data");
                    return new WeatherForecastDto { IsAvailable = false };
                }

                return MapToForecast(apiResponse.Hourly, eventDate, durationHours);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                logger.LogWarning(ex, "Failed to fetch weather forecast from Open-Meteo");
                return new WeatherForecastDto { IsAvailable = false };
            }
        }

        private static WeatherForecastDto MapToForecast(OpenMeteoHourly hourly, DateTimeOffset eventDate, int durationHours)
        {
            if (hourly.Time == null || hourly.Temperature_2m == null || hourly.Time.Count == 0)
            {
                return new WeatherForecastDto { IsAvailable = false };
            }

            // Find the hour index matching event start time
            var eventHour = eventDate.Hour;
            var endHour = Math.Min(eventHour + Math.Max(durationHours, 1), 24);

            // Clamp to available data range
            var startIdx = Math.Clamp(eventHour, 0, hourly.Time.Count - 1);
            var endIdx = Math.Clamp(endHour, startIdx + 1, hourly.Time.Count);

            // Get values for event start hour
            var temp = hourly.Temperature_2m.ElementAtOrDefault(startIdx);
            var precipChance = hourly.Precipitation_probability?.ElementAtOrDefault(startIdx);
            var weatherCode = hourly.Weather_code?.ElementAtOrDefault(startIdx);
            var windSpeed = hourly.Wind_speed_10m?.ElementAtOrDefault(startIdx);

            // Calculate high/low across event duration window
            var windowTemps = hourly.Temperature_2m.Skip(startIdx).Take(endIdx - startIdx).ToList();
            var highTemp = windowTemps.Count > 0 ? windowTemps.Max() : temp;
            var lowTemp = windowTemps.Count > 0 ? windowTemps.Min() : temp;

            // Get max precipitation chance across window
            var windowPrecip = hourly.Precipitation_probability?.Skip(startIdx).Take(endIdx - startIdx).ToList();
            var maxPrecipChance = windowPrecip?.Count > 0 ? windowPrecip.Max() : precipChance;

            return new WeatherForecastDto
            {
                IsAvailable = true,
                Temperature = temp,
                ConditionCode = weatherCode,
                ConditionText = GetConditionText(weatherCode ?? 0),
                PrecipitationChance = maxPrecipChance,
                WindSpeed = windSpeed,
                HighTemperature = highTemp,
                LowTemperature = lowTemp,
            };
        }

        public static string GetConditionText(int wmoCode) => wmoCode switch
        {
            0 => "Clear Sky",
            1 => "Mainly Clear",
            2 => "Partly Cloudy",
            3 => "Overcast",
            45 or 48 => "Foggy",
            51 => "Light Drizzle",
            53 => "Moderate Drizzle",
            55 => "Dense Drizzle",
            61 => "Light Rain",
            63 => "Moderate Rain",
            65 => "Heavy Rain",
            71 => "Light Snow",
            73 => "Moderate Snow",
            75 => "Heavy Snow",
            77 => "Snow Grains",
            80 => "Light Showers",
            81 => "Moderate Showers",
            82 => "Heavy Showers",
            85 => "Light Snow Showers",
            86 => "Heavy Snow Showers",
            95 => "Thunderstorm",
            96 or 99 => "Thunderstorm with Hail",
            _ => "Unknown",
        };
    }

    // Internal models for Open-Meteo JSON deserialization
    internal class OpenMeteoResponse
    {
        public OpenMeteoHourly? Hourly { get; set; }
    }

    internal class OpenMeteoHourly
    {
        public List<string>? Time { get; set; }
        public List<double>? Temperature_2m { get; set; }
        public List<int>? Precipitation_probability { get; set; }
        public List<int>? Weather_code { get; set; }
        public List<double>? Wind_speed_10m { get; set; }
    }
}
