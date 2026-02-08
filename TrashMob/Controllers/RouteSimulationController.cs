namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Identity.Web.Resource;
    using NetTopologySuite.Geometries;
    using TrashMob.Models;
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Generates simulated GPS routes for dev/QA testing. Not available in production.
    /// </summary>
    [Route("api/routes/simulate")]
    public class RouteSimulationController(
        IHostEnvironment env,
        IEventAttendeeRouteManager routeManager,
        IKeyedRepository<Event> eventRepository) : SecureController
    {
        private const double EarthRadiusMeters = 6_371_000;
        private static readonly Random Rng = new();

        /// <summary>
        /// Generates a simulated GPS route around an event's location.
        /// Only available in non-production environments.
        /// </summary>
        /// <param name="eventId">The event to simulate a route for.</param>
        /// <param name="distanceMeters">Target total distance in meters.</param>
        /// <param name="durationMinutes">Target duration in minutes.</param>
        /// <param name="pointCount">Number of GPS waypoints to generate.</param>
        /// <param name="gpsJitterMeters">GPS noise standard deviation in meters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{eventId}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(DisplayEventAttendeeRoute), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GenerateSimulatedRoute(
            Guid eventId,
            [FromQuery] int distanceMeters = 1000,
            [FromQuery] int durationMinutes = 30,
            [FromQuery] int pointCount = 50,
            [FromQuery] int gpsJitterMeters = 3,
            CancellationToken cancellationToken = default)
        {
            if (env.IsProduction())
            {
                return NotFound();
            }

            var eventData = await eventRepository.GetAsync(eventId, cancellationToken);
            if (eventData == null)
            {
                return NotFound($"Event {eventId} not found");
            }

            var centerLat = eventData.Latitude ?? 47.6062;
            var centerLon = eventData.Longitude ?? -122.3321;

            var coordinates = GenerateWalkingLoop(centerLat, centerLon, distanceMeters, pointCount, gpsJitterMeters);

            var factory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var userPath = factory.CreateLineString(coordinates.ToArray());

            var startTime = DateTimeOffset.UtcNow.AddMinutes(-durationMinutes);
            var endTime = DateTimeOffset.UtcNow;

            var route = new EventAttendeeRoute
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                UserId = UserId,
                UserPath = userPath,
                StartTime = startTime,
                EndTime = endTime,
                PrivacyLevel = "EventOnly",
                Notes = "Simulated route for testing",
                BagsCollected = Rng.Next(1, 6),
                WeightCollected = Math.Round((decimal)(Rng.NextDouble() * 10 + 1), 1),
            };

            var created = await routeManager.AddAsync(route, UserId, cancellationToken);

            TrackEvent(nameof(GenerateSimulatedRoute));
            return Ok(created.ToDisplayEventAttendeeRoute());
        }

        private static List<Coordinate> GenerateWalkingLoop(
            double centerLat, double centerLon,
            int totalDistanceMeters, int pointCount, int jitterMeters)
        {
            var stepDistance = (double)totalDistanceMeters / pointCount;
            var bearing = Rng.NextDouble() * 360;
            var coordinates = new List<Coordinate>();

            var currentLat = centerLat;
            var currentLon = centerLon;
            coordinates.Add(new Coordinate(currentLon, currentLat));

            for (var i = 1; i < pointCount; i++)
            {
                var progress = (double)i / pointCount;

                if (progress > 0.8)
                {
                    // Curve back toward start in the final 20%
                    var targetBearing = BearingTo(currentLat, currentLon, centerLat, centerLon);
                    bearing = bearing + (targetBearing - bearing) * 0.3;
                }
                else
                {
                    // Random walk with gentle direction changes
                    bearing += (Rng.NextDouble() - 0.5) * 30;
                }

                var (newLat, newLon) = OffsetLatLon(currentLat, currentLon, bearing, stepDistance);

                // Add GPS jitter
                if (jitterMeters > 0)
                {
                    var jitterLat = (Rng.NextDouble() - 0.5) * 2 * jitterMeters / EarthRadiusMeters * (180 / Math.PI);
                    var jitterLon = jitterLat / Math.Cos(newLat * Math.PI / 180);
                    newLat += jitterLat;
                    newLon += jitterLon;
                }

                coordinates.Add(new Coordinate(newLon, newLat));
                currentLat = newLat;
                currentLon = newLon;
            }

            return coordinates;
        }

        private static (double lat, double lon) OffsetLatLon(double lat, double lon, double bearingDeg, double distanceMeters)
        {
            var latRad = lat * Math.PI / 180;
            var lonRad = lon * Math.PI / 180;
            var bearingRad = bearingDeg * Math.PI / 180;
            var angularDistance = distanceMeters / EarthRadiusMeters;

            var newLatRad = Math.Asin(
                Math.Sin(latRad) * Math.Cos(angularDistance) +
                Math.Cos(latRad) * Math.Sin(angularDistance) * Math.Cos(bearingRad));

            var newLonRad = lonRad + Math.Atan2(
                Math.Sin(bearingRad) * Math.Sin(angularDistance) * Math.Cos(latRad),
                Math.Cos(angularDistance) - Math.Sin(latRad) * Math.Sin(newLatRad));

            return (newLatRad * 180 / Math.PI, newLonRad * 180 / Math.PI);
        }

        private static double BearingTo(double lat1, double lon1, double lat2, double lon2)
        {
            var lat1Rad = lat1 * Math.PI / 180;
            var lat2Rad = lat2 * Math.PI / 180;
            var dLonRad = (lon2 - lon1) * Math.PI / 180;

            var y = Math.Sin(dLonRad) * Math.Cos(lat2Rad);
            var x = Math.Cos(lat1Rad) * Math.Sin(lat2Rad) -
                    Math.Sin(lat1Rad) * Math.Cos(lat2Rad) * Math.Cos(dLonRad);

            return Math.Atan2(y, x) * 180 / Math.PI;
        }
    }
}
