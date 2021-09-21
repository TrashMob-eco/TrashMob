namespace TrashMob.Shared.Persistence
{
    using AzureMapsToolkit.Spatial;
    using AzureMapsToolkit.Timezone;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class MapRepository : IMapRepository
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<MapRepository> logger;
        private const string AzureMapKeyName = "AzureMapsKey";
        private const int MetersPerKilometer = 1000;
        private const int MetersPerMile = 1609;

        public MapRepository(IConfiguration configuration, ILogger<MapRepository> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public string GetMapKey()
        {
            return configuration[AzureMapKeyName];
        }

        public async Task<double> GetDistanceBetweenTwoPoints(Tuple<double, double> pointA, Tuple<double, double> pointB, bool IsMetric = true)
        {
            var azureMaps = new AzureMapsToolkit.AzureMapsServices(GetMapKey());
            var distanceRequest = new GreatCircleDistanceRequest
            {
                Query = $"{pointA.Item1},{pointA.Item2}:{pointB.Item1},{pointB.Item2}",
                Start = new Coordinate() { Lat = pointA.Item1, Lon = pointA.Item2 },
                End = new Coordinate() { Lat = pointB.Item1, Lon = pointB.Item2 }
            };

            logger.LogInformation("Getting distance between two points: {0}", JsonSerializer.Serialize(distanceRequest));

            var response = await azureMaps.GetGreatCircleDistance(distanceRequest).ConfigureAwait(false);

            logger.LogInformation("Response from getting distance between two points: {0}", JsonSerializer.Serialize(response));

            try
            {
                if (response.HttpResponseCode != (int)HttpStatusCode.OK && response.HttpResponseCode != 0)
                {
                    throw new Exception($"Error getting GetGreatCircleDistance: {JsonSerializer.Serialize(response)}");
                }

                var distanceInMeters = (long)response.Result.Result.DistanceInMeters;

                logger.LogInformation("Distance in Meters: {0}", distanceInMeters);

                if (IsMetric)
                {
                    var res = distanceInMeters / MetersPerKilometer;
                    logger.LogInformation("Kilometers : {0}", res);
                    return res;
                }
                else
                {
                    var res = distanceInMeters / MetersPerMile;
                    logger.LogInformation("Miles : {0}", res);
                    return res;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception encountered");
                throw;
            }
        }

        public async Task<string> GetTimeForPoint(Tuple<double, double> pointA, DateTimeOffset dateTimeOffset)
        {
            var azureMaps = new AzureMapsToolkit.AzureMapsServices(GetMapKey());

            if (azureMaps == null)
            {
                logger.LogError("Failed to get instance of azuremaps.");
                throw new Exception("Failed to get instance of azuremaps");
            }

            var timezoneRequest = new TimeZoneRequest
            {
                Query = $"{pointA.Item1},{pointA.Item2}",
                TimeStamp = dateTimeOffset.ToString("O")
            };

            logger.LogInformation("Getting time for timezoneRequest: {0}", JsonSerializer.Serialize(timezoneRequest));

            var response = await azureMaps.GetTimezoneByCoordinates(timezoneRequest).ConfigureAwait(false);

            logger.LogInformation("Response from getting time for timezoneRequest: {0}", JsonSerializer.Serialize(response));

            if (response.HttpResponseCode != (int)HttpStatusCode.OK && response.HttpResponseCode != 0)
            {
                throw new Exception($"Error getting timezonebycoordinates: {response}");
            }

            return response?.Result?.TimeZones[0]?.ReferenceTime?.WallTime;
        }
    }
}
