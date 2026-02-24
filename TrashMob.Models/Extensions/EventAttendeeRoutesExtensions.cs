namespace TrashMob.Models.Extensions
{
    using NetTopologySuite.Geometries;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Provides extension methods for converting EventAttendeeRoute to DisplayEventAttendeeRoute.
    /// </summary>
    public static class EventAttendeeRoutesExtensions
    {
        /// <summary>
        /// Converts an EventAttendeeRoute to a DisplayEventAttendeeRoute with sortable location data.
        /// </summary>
        /// <param name="eventAttendeeRoute">The event attendee route to convert.</param>
        /// <returns>A DisplayEventAttendeeRoute with locations extracted from the geometric path.</returns>
        public static DisplayEventAttendeeRoute ToDisplayEventAttendeeRoute(this EventAttendeeRoute eventAttendeeRoute)
        {
            var density = CalculateDensity(eventAttendeeRoute.BagsCollected,
                eventAttendeeRoute.WeightCollected, eventAttendeeRoute.WeightUnitId,
                eventAttendeeRoute.TotalDistanceMeters);

            return new DisplayEventAttendeeRoute
            {
                Id = eventAttendeeRoute.Id,
                EventId = eventAttendeeRoute.EventId,
                UserId = eventAttendeeRoute.UserId,
                EndTime = eventAttendeeRoute.EndTime,
                StartTime = eventAttendeeRoute.StartTime,
                TotalDistanceMeters = eventAttendeeRoute.TotalDistanceMeters,
                DurationMinutes = eventAttendeeRoute.DurationMinutes,
                PrivacyLevel = eventAttendeeRoute.PrivacyLevel,
                IsTrimmed = eventAttendeeRoute.IsTrimmed,
                TrimStartMeters = eventAttendeeRoute.TrimStartMeters,
                TrimEndMeters = eventAttendeeRoute.TrimEndMeters,
                BagsCollected = eventAttendeeRoute.BagsCollected,
                WeightCollected = eventAttendeeRoute.WeightCollected,
                WeightUnitId = eventAttendeeRoute.WeightUnitId,
                Notes = eventAttendeeRoute.Notes,
                ExpiresDate = eventAttendeeRoute.ExpiresDate,
                IsTimeTrimmed = eventAttendeeRoute.IsTimeTrimmed,
                OriginalEndTime = eventAttendeeRoute.OriginalEndTime,
                OriginalTotalDistanceMeters = eventAttendeeRoute.OriginalTotalDistanceMeters,
                OriginalDurationMinutes = eventAttendeeRoute.OriginalDurationMinutes,
                DensityGramsPerMeter = density,
                DensityColor = GetDensityColor(density),
                Locations = GetSortedLocations(eventAttendeeRoute.UserPath)
            };
        }

        public static DisplayAnonymizedRoute ToDisplayAnonymizedRoute(this EventAttendeeRoute eventAttendeeRoute)
        {
            var density = CalculateDensity(eventAttendeeRoute.BagsCollected,
                eventAttendeeRoute.WeightCollected, eventAttendeeRoute.WeightUnitId,
                eventAttendeeRoute.TotalDistanceMeters);

            return new DisplayAnonymizedRoute
            {
                Id = eventAttendeeRoute.Id,
                EventId = eventAttendeeRoute.EventId,
                StartTime = eventAttendeeRoute.StartTime,
                EndTime = eventAttendeeRoute.EndTime,
                TotalDistanceMeters = eventAttendeeRoute.TotalDistanceMeters,
                DurationMinutes = eventAttendeeRoute.DurationMinutes,
                BagsCollected = eventAttendeeRoute.BagsCollected,
                WeightCollected = eventAttendeeRoute.WeightCollected,
                WeightUnitId = eventAttendeeRoute.WeightUnitId,
                DensityGramsPerMeter = density,
                DensityColor = GetDensityColor(density),
                Locations = GetSortedLocations(eventAttendeeRoute.UserPath)
            };
        }

        private static List<SortableLocation> GetSortedLocations(Geometry userPath)
        {
            var locations = new List<SortableLocation>();
            var order = 0;
            foreach (var coordinate in userPath.Coordinates)
            {
                locations.Add(new SortableLocation
                {
                    Latitude = coordinate.Y,
                    Longitude = coordinate.X,
                    SortOrder = order++
                });
            }

            return locations;
        }

        private const decimal GramsPerPound = 453.592m;
        private const decimal GramsPerKilogram = 1000m;

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
    }
}
