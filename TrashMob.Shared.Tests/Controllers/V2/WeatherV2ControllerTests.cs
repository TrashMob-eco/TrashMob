namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Services;
    using Xunit;

    public class WeatherV2ControllerTests
    {
        private readonly Mock<IWeatherService> weatherService = new();
        private readonly Mock<ILogger<WeatherV2Controller>> logger = new();
        private readonly WeatherV2Controller controller;

        public WeatherV2ControllerTests()
        {
            controller = new WeatherV2Controller(weatherService.Object, logger.Object);
        }

        [Fact]
        public async Task GetForecast_ValidCoordinates_ReturnsOkWithForecast()
        {
            var forecast = new WeatherForecastDto
            {
                IsAvailable = true,
                Temperature = 65.0,
                ConditionCode = 2,
                ConditionText = "Partly Cloudy",
                PrecipitationChance = 20,
                WindSpeed = 8.5,
                HighTemperature = 70.0,
                LowTemperature = 58.0,
            };

            weatherService
                .Setup(s => s.GetForecastAsync(47.61, -122.33, It.IsAny<DateTimeOffset>(), 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(forecast);

            var result = await controller.GetForecast(47.61, -122.33, DateTimeOffset.UtcNow.AddDays(1), 2, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<WeatherForecastDto>(okResult.Value);
            Assert.True(dto.IsAvailable);
            Assert.Equal(65.0, dto.Temperature);
            Assert.Equal("Partly Cloudy", dto.ConditionText);
            Assert.Equal(20, dto.PrecipitationChance);
        }

        [Fact]
        public async Task GetForecast_UnavailableForecast_ReturnsOkWithIsAvailableFalse()
        {
            weatherService
                .Setup(s => s.GetForecastAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WeatherForecastDto { IsAvailable = false });

            var result = await controller.GetForecast(47.61, -122.33, DateTimeOffset.UtcNow.AddDays(30), 2, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<WeatherForecastDto>(okResult.Value);
            Assert.False(dto.IsAvailable);
        }

        [Theory]
        [InlineData(91, 0)]
        [InlineData(-91, 0)]
        [InlineData(0, 181)]
        [InlineData(0, -181)]
        public async Task GetForecast_InvalidCoordinates_ReturnsBadRequest(double lat, double lng)
        {
            var result = await controller.GetForecast(lat, lng, DateTimeOffset.UtcNow.AddDays(1), 2, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
