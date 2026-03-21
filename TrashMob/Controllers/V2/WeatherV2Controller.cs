namespace TrashMob.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Services;

    /// <summary>
    /// V2 controller for weather forecast data.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/weather")]
    public class WeatherV2Controller(
        IWeatherService weatherService,
        ILogger<WeatherV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets a weather forecast for a specific location and date.
        /// </summary>
        /// <param name="lat">Latitude of the event location.</param>
        /// <param name="lng">Longitude of the event location.</param>
        /// <param name="date">Event date/time in ISO 8601 format.</param>
        /// <param name="durationHours">Event duration in hours (default 2).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Weather forecast for the specified location and time.</returns>
        [HttpGet("forecast")]
        [ProducesResponseType(typeof(WeatherForecastDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetForecast(
            [FromQuery] double lat,
            [FromQuery] double lng,
            [FromQuery] DateTimeOffset date,
            [FromQuery] int durationHours = 2,
            CancellationToken cancellationToken = default)
        {
            if (lat is < -90 or > 90 || lng is < -180 or > 180)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid coordinates",
                    Detail = "Latitude must be between -90 and 90, longitude between -180 and 180.",
                });
            }

            logger.LogInformation("V2 GetWeatherForecast Lat={Latitude}, Lng={Longitude}, Date={Date}",
                lat, lng, date);

            var forecast = await weatherService.GetForecastAsync(lat, lng, date, durationHours, cancellationToken);
            return Ok(forecast);
        }
    }
}
