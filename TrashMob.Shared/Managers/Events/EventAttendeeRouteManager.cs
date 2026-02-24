namespace TrashMob.Shared.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NetTopologySuite.Geometries;
    using TrashMob.Models;
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages event attendee GPS routes recorded during cleanup events.
    /// </summary>
    public class EventAttendeeRouteManager(
        IKeyedRepository<EventAttendeeRoute> eventAttendeeRouteRepository)
        : KeyedManager<EventAttendeeRoute>(eventAttendeeRouteRepository), IBaseManager<EventAttendeeRoute>,
            IEventAttendeeRouteManager
    {

        /// <inheritdoc />
        public override async Task<IEnumerable<EventAttendeeRoute>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get().Where(p => p.EventId == parentId)
                    .Include(p => p.User)
                    .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<EventAttendeeRoute> AddAsync(EventAttendeeRoute instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            CalculateRouteMetrics(instance);
            ApplyDefaultTrim(instance);

            return await base.AddAsync(instance, userId, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<EventAttendeeRoute> AddAsync(EventAttendeeRoute instance,
            CancellationToken cancellationToken = default)
        {
            CalculateRouteMetrics(instance);
            ApplyDefaultTrim(instance);

            return await base.AddAsync(instance, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DisplayAnonymizedRoute>> GetAnonymizedRoutesForEventAsync(Guid eventId,
            CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            var routes = await Repository.Get()
                .Where(r => r.EventId == eventId
                    && r.PrivacyLevel != "Private"
                    && (r.ExpiresDate == null || r.ExpiresDate > now))
                .ToListAsync(cancellationToken);

            return routes.Select(r => r.ToDisplayAnonymizedRoute());
        }

        /// <inheritdoc />
        public async Task<DisplayEventRouteStats> GetEventRouteStatsAsync(Guid eventId,
            CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            var routes = await Repository.Get()
                .Where(r => r.EventId == eventId
                    && r.PrivacyLevel != "Private"
                    && (r.ExpiresDate == null || r.ExpiresDate > now))
                .ToListAsync(cancellationToken);

            var densities = routes
                .Select(r => CalculateDensity(r.BagsCollected, r.WeightCollected, r.WeightUnitId, r.TotalDistanceMeters))
                .Where(d => d.HasValue)
                .Select(d => d!.Value)
                .ToList();

            return new DisplayEventRouteStats
            {
                EventId = eventId,
                TotalRoutes = routes.Count,
                TotalDistanceMeters = routes.Sum(r => (long)r.TotalDistanceMeters),
                TotalDurationMinutes = routes.Sum(r => (long)r.DurationMinutes),
                UniqueContributors = routes.Select(r => r.UserId).Distinct().Count(),
                TotalBagsCollected = routes.Sum(r => r.BagsCollected ?? 0),
                TotalWeightCollected = Math.Round(ConvertWeightsToPounds(routes), 1),
                TotalWeightUnitId = (int)WeightUnitEnum.Pound,
                CoverageAreaSquareMeters = CalculateCoverageArea(routes),
                AverageDensityGramsPerMeter = densities.Count > 0 ? Math.Round(densities.Average(), 1) : null,
                MaxDensityGramsPerMeter = densities.Count > 0 ? Math.Round(densities.Max(), 1) : null,
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DisplayUserRouteHistory>> GetUserRouteHistoryAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            var routes = await Repository.Get()
                .Where(r => r.UserId == userId)
                .Include(r => r.Event)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync(cancellationToken);

            return routes.Select(r =>
            {
                var density = CalculateDensity(r.BagsCollected, r.WeightCollected, r.WeightUnitId, r.TotalDistanceMeters);
                return new DisplayUserRouteHistory
                {
                    RouteId = r.Id,
                    EventId = r.EventId,
                    EventName = r.Event?.Name,
                    EventDate = r.Event?.EventDate ?? r.StartTime,
                    TotalDistanceMeters = r.TotalDistanceMeters,
                    DurationMinutes = r.DurationMinutes,
                    PrivacyLevel = r.PrivacyLevel,
                    BagsCollected = r.BagsCollected,
                    WeightCollected = r.WeightCollected,
                    WeightUnitId = r.WeightUnitId,
                    EventLatitude = r.Event?.Latitude ?? 0,
                    EventLongitude = r.Event?.Longitude ?? 0,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    IsTimeTrimmed = r.IsTimeTrimmed,
                    DensityGramsPerMeter = density,
                    DensityColor = GetDensityColor(density),
                    Locations = ExtractLocations(r.UserPath),
                };
            });
        }

        /// <inheritdoc />
        public async Task<ServiceResult<EventAttendeeRoute>> UpdateRouteMetadataAsync(Guid routeId, Guid userId,
            UpdateRouteMetadataRequest request, CancellationToken cancellationToken = default)
        {
            var route = await Repo.GetAsync(routeId, cancellationToken);

            if (route is null)
            {
                return ServiceResult<EventAttendeeRoute>.Failure("Route not found.");
            }

            if (route.UserId != userId)
            {
                return ServiceResult<EventAttendeeRoute>.Failure("You can only update your own routes.");
            }

            var validPrivacyLevels = new[] { "Private", "EventOnly", "Public" };
            if (!string.IsNullOrWhiteSpace(request.PrivacyLevel) && !validPrivacyLevels.Contains(request.PrivacyLevel))
            {
                return ServiceResult<EventAttendeeRoute>.Failure("Invalid privacy level. Must be Private, EventOnly, or Public.");
            }

            if (!string.IsNullOrWhiteSpace(request.PrivacyLevel))
            {
                route.PrivacyLevel = request.PrivacyLevel;
            }

            route.TrimStartMeters = request.TrimStartMeters;
            route.TrimEndMeters = request.TrimEndMeters;
            route.Notes = request.Notes;
            route.BagsCollected = request.BagsCollected;
            route.WeightCollected = request.WeightCollected;
            route.WeightUnitId = request.WeightUnitId;

            if (route.PrivacyLevel == "Public" && route.ExpiresDate is null)
            {
                route.ExpiresDate = DateTimeOffset.UtcNow.AddYears(2);
            }
            else if (route.PrivacyLevel != "Public")
            {
                route.ExpiresDate = null;
            }

            ApplyTrim(route);

            var updated = await Repository.UpdateAsync(route);
            return ServiceResult<EventAttendeeRoute>.Success(updated);
        }

        /// <inheritdoc />
        public async Task<EventSummaryPrefill> GetEventSummaryPrefillAsync(Guid eventId,
            int targetWeightUnitId, CancellationToken cancellationToken = default)
        {
            var routes = await Repository.Get()
                .Where(r => r.EventId == eventId)
                .ToListAsync(cancellationToken);

            if (routes.Count == 0)
            {
                return new EventSummaryPrefill { HasRouteData = false };
            }

            var totalWeightInPounds = ConvertWeightsToPounds(routes);

            // Convert from pounds to target unit
            var totalWeight = targetWeightUnitId == (int)WeightUnitEnum.Kilogram
                ? totalWeightInPounds * PoundsToKilogramsMultiplier
                : totalWeightInPounds;

            return new EventSummaryPrefill
            {
                NumberOfBags = routes.Sum(r => r.BagsCollected ?? 0),
                PickedWeight = Math.Round(totalWeight, 1),
                PickedWeightUnitId = targetWeightUnitId,
                DurationInMinutes = (int)routes.Sum(r => (long)r.DurationMinutes),
                ActualNumberOfAttendees = routes.Select(r => r.UserId).Distinct().Count(),
                HasRouteData = true,
            };
        }

        /// <inheritdoc />
        public async Task<ServiceResult<EventAttendeeRoute>> TrimRouteTimeAsync(Guid routeId, Guid userId,
            TrimRouteTimeRequest request, CancellationToken cancellationToken = default)
        {
            var route = await Repository.Get()
                .Include(r => r.RoutePoints)
                .FirstOrDefaultAsync(r => r.Id == routeId, cancellationToken);

            if (route is null)
            {
                return ServiceResult<EventAttendeeRoute>.Failure("Route not found.");
            }

            if (route.UserId != userId)
            {
                return ServiceResult<EventAttendeeRoute>.Failure("You can only trim your own routes.");
            }

            if (request.NewEndTime <= route.StartTime)
            {
                return ServiceResult<EventAttendeeRoute>.Failure("New end time must be after the route start time.");
            }

            if (request.NewEndTime >= route.EndTime)
            {
                return ServiceResult<EventAttendeeRoute>.Failure("New end time must be before the current end time.");
            }

            // Preserve originals on first trim
            route.OriginalEndTime ??= route.EndTime;
            route.OriginalTotalDistanceMeters ??= route.TotalDistanceMeters;
            route.OriginalDurationMinutes ??= route.DurationMinutes;

            // Filter RoutePoints to those before new end time
            var keptPoints = route.RoutePoints
                .Where(rp => rp.Timestamp <= request.NewEndTime)
                .OrderBy(rp => rp.Timestamp)
                .ToList();

            if (keptPoints.Count < 2)
            {
                return ServiceResult<EventAttendeeRoute>.Failure("Trimming to this time would leave fewer than 2 GPS points.");
            }

            // Rebuild the LineString from kept points
            var factory = route.UserPath?.Factory ?? NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            var coordinates = keptPoints.Select(p => new Coordinate(p.Longitude, p.Latitude)).ToArray();
            var newPath = factory.CreateLineString(coordinates);

            route.UserPath = newPath;
            route.EndTime = request.NewEndTime;
            route.TotalDistanceMeters = (int)CalculateHaversineDistance(newPath);
            route.DurationMinutes = (int)(route.EndTime - route.StartTime).TotalMinutes;
            route.IsTimeTrimmed = true;

            // Re-apply privacy trim on the new path
            ApplyTrim(route);

            var updated = await Repository.UpdateAsync(route);
            return ServiceResult<EventAttendeeRoute>.Success(updated);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<EventAttendeeRoute>> RestoreRouteTimeAsync(Guid routeId, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var route = await Repository.Get()
                .Include(r => r.RoutePoints)
                .FirstOrDefaultAsync(r => r.Id == routeId, cancellationToken);

            if (route is null)
            {
                return ServiceResult<EventAttendeeRoute>.Failure("Route not found.");
            }

            if (route.UserId != userId)
            {
                return ServiceResult<EventAttendeeRoute>.Failure("You can only restore your own routes.");
            }

            if (!route.IsTimeTrimmed)
            {
                return ServiceResult<EventAttendeeRoute>.Failure("This route has not been time-trimmed.");
            }

            // Rebuild the full path from ALL RoutePoints
            var allPoints = route.RoutePoints
                .OrderBy(rp => rp.Timestamp)
                .ToList();

            if (allPoints.Count < 2)
            {
                return ServiceResult<EventAttendeeRoute>.Failure("Cannot restore — insufficient route points.");
            }

            var factory = route.UserPath?.Factory ?? NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            var coordinates = allPoints.Select(p => new Coordinate(p.Longitude, p.Latitude)).ToArray();
            route.UserPath = factory.CreateLineString(coordinates);

            // Restore original values
            route.EndTime = route.OriginalEndTime!.Value;
            route.TotalDistanceMeters = route.OriginalTotalDistanceMeters!.Value;
            route.DurationMinutes = route.OriginalDurationMinutes!.Value;

            // Clear time-trim state
            route.OriginalEndTime = null;
            route.OriginalTotalDistanceMeters = null;
            route.OriginalDurationMinutes = null;
            route.IsTimeTrimmed = false;

            // Re-apply privacy trim on the restored path
            ApplyTrim(route);

            var updated = await Repository.UpdateAsync(route);
            return ServiceResult<EventAttendeeRoute>.Success(updated);
        }

        private const decimal KilogramsToPoundsMultiplier = 2.20462m;
        private const decimal PoundsToKilogramsMultiplier = 0.453592m;
        private const decimal GramsPerPound = 453.592m;
        private const decimal GramsPerKilogram = 1000m;

        private static decimal ConvertWeightsToPounds(List<EventAttendeeRoute> routes)
        {
            var totalWeightInPounds = 0m;
            foreach (var route in routes)
            {
                if (route.WeightCollected.HasValue && route.WeightCollected.Value > 0)
                {
                    totalWeightInPounds += route.WeightUnitId == (int)WeightUnitEnum.Kilogram
                        ? route.WeightCollected.Value * KilogramsToPoundsMultiplier
                        : route.WeightCollected.Value;
                }
            }

            return totalWeightInPounds;
        }

        private static List<SortableLocation> ExtractLocations(Geometry userPath)
        {
            if (userPath is not LineString lineString || lineString.NumPoints < 2)
            {
                return [];
            }

            var locations = new List<SortableLocation>();
            var order = 0;
            foreach (var coordinate in lineString.Coordinates)
            {
                locations.Add(new SortableLocation
                {
                    Latitude = coordinate.Y,
                    Longitude = coordinate.X,
                    SortOrder = order++,
                });
            }

            return locations;
        }

        private const double GridCellSizeMeters = 25.0;
        private const double MetersPerDegreeLatitude = 111_000.0;

        private static double CalculateCoverageArea(List<EventAttendeeRoute> routes)
        {
            var uniqueCells = new HashSet<(int, int)>();

            foreach (var route in routes)
            {
                if (route.UserPath is not LineString lineString)
                {
                    continue;
                }

                foreach (var coord in lineString.Coordinates)
                {
                    var latMeters = coord.Y * MetersPerDegreeLatitude;
                    var lonMeters = coord.X * MetersPerDegreeLatitude * Math.Cos(coord.Y * Math.PI / 180);
                    var cellKey = ((int)Math.Floor(latMeters / GridCellSizeMeters), (int)Math.Floor(lonMeters / GridCellSizeMeters));
                    uniqueCells.Add(cellKey);
                }
            }

            return uniqueCells.Count * GridCellSizeMeters * GridCellSizeMeters;
        }

        private static void CalculateRouteMetrics(EventAttendeeRoute route)
        {
            if (route.UserPath is LineString lineString && lineString.NumPoints >= 2)
            {
                route.TotalDistanceMeters = (int)CalculateHaversineDistance(lineString);
            }

            if (route.EndTime > route.StartTime)
            {
                route.DurationMinutes = (int)(route.EndTime - route.StartTime).TotalMinutes;
            }
        }

        private static void ApplyDefaultTrim(EventAttendeeRoute route)
        {
            if (route.TrimStartMeters == 0)
            {
                route.TrimStartMeters = 100;
            }

            if (route.TrimEndMeters == 0)
            {
                route.TrimEndMeters = 100;
            }

            ApplyTrim(route);
        }

        private static void ApplyTrim(EventAttendeeRoute route)
        {
            if (route.UserPath is not LineString lineString || lineString.NumPoints < 2)
            {
                return;
            }

            var trimmedCoords = TrimLineString(lineString, route.TrimStartMeters, route.TrimEndMeters);

            if (trimmedCoords.Count >= 2)
            {
                var factory = route.UserPath.Factory;
                route.UserPath = factory.CreateLineString(trimmedCoords.ToArray());
                route.IsTrimmed = true;
            }
        }

        private static List<Coordinate> TrimLineString(LineString lineString, int trimStartMeters, int trimEndMeters)
        {
            var coords = lineString.Coordinates;
            var totalDistance = CalculateHaversineDistance(lineString);

            if (trimStartMeters + trimEndMeters >= totalDistance)
            {
                return coords.ToList();
            }

            List<Coordinate> result = [];
            var cumulativeDistance = 0.0;

            for (var i = 0; i < coords.Length; i++)
            {
                if (i > 0)
                {
                    cumulativeDistance += HaversineDistance(coords[i - 1], coords[i]);
                }

                if (cumulativeDistance >= trimStartMeters && cumulativeDistance <= totalDistance - trimEndMeters)
                {
                    result.Add(coords[i]);
                }
            }

            return result;
        }

        private static double CalculateHaversineDistance(LineString lineString)
        {
            var totalDistance = 0.0;
            var coords = lineString.Coordinates;

            for (var i = 1; i < coords.Length; i++)
            {
                totalDistance += HaversineDistance(coords[i - 1], coords[i]);
            }

            return totalDistance;
        }

        private static double HaversineDistance(Coordinate coord1, Coordinate coord2)
        {
            const double earthRadiusMeters = 6371000;

            var lat1 = coord1.Y * Math.PI / 180;
            var lat2 = coord2.Y * Math.PI / 180;
            var deltaLat = (coord2.Y - coord1.Y) * Math.PI / 180;
            var deltaLon = (coord2.X - coord1.X) * Math.PI / 180;

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadiusMeters * c;
        }

        #region Density Calculation

        private static double? CalculateDensity(int? bags, decimal? weight, int? weightUnitId, int distanceMeters)
        {
            if (distanceMeters <= 0)
            {
                return null;
            }

            if (weight.HasValue && weight.Value > 0)
            {
                var grams = weightUnitId == (int)WeightUnitEnum.Kilogram
                    ? weight.Value * GramsPerKilogram
                    : weight.Value * GramsPerPound;
                return (double)(grams / distanceMeters);
            }

            if (bags.HasValue && bags.Value > 0)
            {
                var distanceKm = distanceMeters / 1000.0;
                return distanceKm > 0 ? bags.Value / distanceKm : null;
            }

            return null;
        }

        private static string GetDensityColor(double? density) => density switch
        {
            null or < 0.01 => "#9E9E9E",
            < 5 => "#4CAF50",
            < 15 => "#8BC34A",
            < 30 => "#FFC107",
            < 60 => "#FF9800",
            < 120 => "#FF5722",
            _ => "#F44336",
        };


        #endregion
    }
}
