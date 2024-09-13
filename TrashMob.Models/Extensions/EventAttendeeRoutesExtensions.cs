namespace TrashMob.Models.Extensions
{
    using NetTopologySuite.Geometries;
    using TrashMob.Models.Poco;

    public static class EventAttendeeRoutesExtensions
    {
        public static DisplayEventAttendeeRoute ToDisplayEventAttendeeRoute(this EventAttendeeRoute eventAttendeeRoute)
        {
            return new DisplayEventAttendeeRoute
            {
                Id = eventAttendeeRoute.Id,
                EventId = eventAttendeeRoute.EventId,
                UserId = eventAttendeeRoute.UserId,
                EndTime = eventAttendeeRoute.EndTime,
                StartTime = eventAttendeeRoute.StartTime,
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
