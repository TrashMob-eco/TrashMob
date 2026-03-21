#nullable enable

namespace TrashMob.Shared.Tests.Services
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Shared.Services;
    using Xunit;

    public class OpenMeteoWeatherServiceTests
    {
        [Theory]
        [InlineData(0, "Clear Sky")]
        [InlineData(1, "Mainly Clear")]
        [InlineData(2, "Partly Cloudy")]
        [InlineData(3, "Overcast")]
        [InlineData(45, "Foggy")]
        [InlineData(48, "Foggy")]
        [InlineData(61, "Light Rain")]
        [InlineData(63, "Moderate Rain")]
        [InlineData(65, "Heavy Rain")]
        [InlineData(71, "Light Snow")]
        [InlineData(95, "Thunderstorm")]
        [InlineData(96, "Thunderstorm with Hail")]
        [InlineData(999, "Unknown")]
        public void GetConditionText_ReturnsExpectedText(int wmoCode, string expected)
        {
            var result = OpenMeteoWeatherService.GetConditionText(wmoCode);
            Assert.Equal(expected, result);
        }

        private static OpenMeteoWeatherService CreateService(HttpClient httpClient, IMemoryCache? cache = null)
        {
            cache ??= new MemoryCache(new MemoryCacheOptions());
            var logger = new Mock<ILogger<OpenMeteoWeatherService>>();
            return new OpenMeteoWeatherService(httpClient, cache, logger.Object);
        }

        private static HttpClient CreateMockHttpClient(HttpStatusCode statusCode, object? responseBody = null)
        {
            var handler = new MockHttpMessageHandler(statusCode,
                responseBody != null ? JsonSerializer.Serialize(responseBody) : "");
            return new HttpClient(handler);
        }

        [Fact]
        public async Task GetForecastAsync_PastEvent_ReturnsUnavailable()
        {
            var service = CreateService(new HttpClient());
            var result = await service.GetForecastAsync(47.6, -122.3, DateTimeOffset.UtcNow.AddDays(-1), 2);

            Assert.False(result.IsAvailable);
        }

        [Fact]
        public async Task GetForecastAsync_EventBeyond16Days_ReturnsUnavailable()
        {
            var service = CreateService(new HttpClient());
            var result = await service.GetForecastAsync(47.6, -122.3, DateTimeOffset.UtcNow.AddDays(17), 2);

            Assert.False(result.IsAvailable);
        }

        [Fact]
        public async Task GetForecastAsync_ValidResponse_MapsCorrectly()
        {
            // Build a full 24-hour response so the hour index mapping works correctly
            var times = new string[24];
            var temps = new double[24];
            var precip = new int[24];
            var codes = new int[24];
            var winds = new double[24];
            for (var i = 0; i < 24; i++)
            {
                times[i] = $"2026-03-22T{i:D2}:00";
                temps[i] = 50.0;
                precip[i] = 0;
                codes[i] = 0;
                winds[i] = 3.0;
            }

            // Set specific values at hours 8-10 (event window)
            temps[8] = 55.0;
            temps[9] = 60.0;
            temps[10] = 58.0;
            precip[8] = 10;
            precip[9] = 30;
            precip[10] = 20;
            codes[8] = 2;
            codes[9] = 2;
            codes[10] = 3;
            winds[8] = 5.0;
            winds[9] = 8.0;
            winds[10] = 6.0;

            var apiResponse = new
            {
                hourly = new
                {
                    time = times,
                    temperature_2m = temps,
                    precipitation_probability = precip,
                    weather_code = codes,
                    wind_speed_10m = winds,
                },
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, apiResponse);
            var service = CreateService(httpClient);

            // Event tomorrow at 8am for 2 hours → indices 8-10
            var eventDate = DateTimeOffset.UtcNow.Date.AddDays(1).AddHours(8);
            var result = await service.GetForecastAsync(47.6, -122.3, eventDate, 2);

            Assert.True(result.IsAvailable);
            Assert.Equal(55.0, result.Temperature);
            Assert.Equal(2, result.ConditionCode);
            Assert.Equal("Partly Cloudy", result.ConditionText);
            Assert.Equal(30, result.PrecipitationChance); // Max across window
            Assert.Equal(5.0, result.WindSpeed);
            Assert.Equal(60.0, result.HighTemperature);
            Assert.Equal(55.0, result.LowTemperature);
        }

        [Fact]
        public async Task GetForecastAsync_ApiReturns500_ReturnsUnavailable()
        {
            var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError);
            var service = CreateService(httpClient);

            var result = await service.GetForecastAsync(47.6, -122.3, DateTimeOffset.UtcNow.AddDays(1), 2);

            Assert.False(result.IsAvailable);
        }

        [Fact]
        public async Task GetForecastAsync_EmptyHourlyData_ReturnsUnavailable()
        {
            var apiResponse = new
            {
                hourly = new
                {
                    time = Array.Empty<string>(),
                    temperature_2m = Array.Empty<double>(),
                    precipitation_probability = Array.Empty<int>(),
                    weather_code = Array.Empty<int>(),
                    wind_speed_10m = Array.Empty<double>(),
                },
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, apiResponse);
            var service = CreateService(httpClient);

            var result = await service.GetForecastAsync(47.6, -122.3, DateTimeOffset.UtcNow.AddDays(1), 2);

            Assert.False(result.IsAvailable);
        }

        [Fact]
        public async Task GetForecastAsync_CachesResult()
        {
            var apiResponse = new
            {
                hourly = new
                {
                    time = new[] { "2026-03-22T10:00" },
                    temperature_2m = new[] { 65.0 },
                    precipitation_probability = new[] { 0 },
                    weather_code = new[] { 0 },
                    wind_speed_10m = new[] { 3.0 },
                },
            };

            var callCount = 0;
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK,
                JsonSerializer.Serialize(apiResponse), () => callCount++);
            var httpClient = new HttpClient(handler);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = CreateService(httpClient, cache);

            var eventDate = DateTimeOffset.UtcNow.Date.AddDays(1).AddHours(10);

            // First call hits API
            await service.GetForecastAsync(47.6, -122.3, eventDate, 2);
            Assert.Equal(1, callCount);

            // Second call with same coordinates + date should use cache
            await service.GetForecastAsync(47.6, -122.3, eventDate, 2);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public async Task GetForecastAsync_RoundsCoordinatesForCache()
        {
            var apiResponse = new
            {
                hourly = new
                {
                    time = new[] { "2026-03-22T10:00" },
                    temperature_2m = new[] { 65.0 },
                    precipitation_probability = new[] { 0 },
                    weather_code = new[] { 0 },
                    wind_speed_10m = new[] { 3.0 },
                },
            };

            var callCount = 0;
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK,
                JsonSerializer.Serialize(apiResponse), () => callCount++);
            var httpClient = new HttpClient(handler);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = CreateService(httpClient, cache);

            var eventDate = DateTimeOffset.UtcNow.Date.AddDays(1).AddHours(10);

            // Slightly different coordinates should share cache (rounded to 2 decimals)
            await service.GetForecastAsync(47.60123, -122.33456, eventDate, 2);
            await service.GetForecastAsync(47.60456, -122.33123, eventDate, 2);
            Assert.Equal(1, callCount);
        }
    }

    /// <summary>
    /// Simple HttpMessageHandler mock for testing HTTP calls.
    /// </summary>
    internal class MockHttpMessageHandler(HttpStatusCode statusCode, string responseBody, Action? onSend = null) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            onSend?.Invoke();
            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseBody, System.Text.Encoding.UTF8, "application/json"),
            });
        }
    }
}
