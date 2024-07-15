namespace TrashMob.Models.Extensions
{
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries;
    using TrashMob.Models.Poco;

    public static class DisplayEventAttendeeRoutesExtensions
    {
        public static EventAttendeeRoute ToEventAttendeeRoute(this DisplayEventAttendeeRoute displayEventAttendeeRoute)
        {
            return new EventAttendeeRoute
            {
                Id = displayEventAttendeeRoute.Id,
                EventId = displayEventAttendeeRoute.EventId,
                UserId = displayEventAttendeeRoute.UserId,
                EndTime = displayEventAttendeeRoute.EndTime,
                StartTime = displayEventAttendeeRoute.StartTime,
                UserPath = GetUserPath(displayEventAttendeeRoute.Locations)
            };
        }

        private static LineString GetUserPath(List<SortableLocation> sortableLocations)
        {
            var coordinates = new List<Coordinate>();

            foreach (var location in sortableLocations.OrderBy(x => x.SortOrder))
            {
                coordinates.Add(new Coordinate(location.Longitude, location.Latitude));                
            }

            return new LineString(coordinates.ToArray());
        }
    }
}
