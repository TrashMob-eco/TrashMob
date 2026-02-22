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
                Locations = GetSortedLocations(eventAttendeeRoute.UserPath)
            };
        }

        public static DisplayAnonymizedRoute ToDisplayAnonymizedRoute(this EventAttendeeRoute eventAttendeeRoute)
        {
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
    }
}
