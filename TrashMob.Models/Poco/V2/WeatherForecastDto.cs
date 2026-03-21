#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// Weather forecast for a specific location and time window.
    /// </summary>
    public class WeatherForecastDto
    {
        /// <summary>
        /// Whether forecast data is available (false if event date is beyond forecast range).
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Temperature in Fahrenheit at event start time.
        /// </summary>
        public double? Temperature { get; set; }

        /// <summary>
        /// WMO weather condition code.
        /// </summary>
        public int? ConditionCode { get; set; }

        /// <summary>
        /// Human-readable weather condition text (e.g., "Partly Cloudy", "Rain").
        /// </summary>
        public string? ConditionText { get; set; }

        /// <summary>
        /// Precipitation probability as a percentage (0-100).
        /// </summary>
        public int? PrecipitationChance { get; set; }

        /// <summary>
        /// Wind speed in miles per hour.
        /// </summary>
        public double? WindSpeed { get; set; }

        /// <summary>
        /// High temperature for the event time window in Fahrenheit.
        /// </summary>
        public double? HighTemperature { get; set; }

        /// <summary>
        /// Low temperature for the event time window in Fahrenheit.
        /// </summary>
        public double? LowTemperature { get; set; }
    }
}
