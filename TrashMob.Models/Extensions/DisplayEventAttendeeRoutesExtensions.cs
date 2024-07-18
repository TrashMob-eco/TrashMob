namespace TrashMob.Models.Extensions
{
    using System.Collections.Generic;
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries;
    using TrashMob.Models.Poco;

    public static class DisplayEventAttendeeRoutesExtensions
    {
        public static EventAttendeeRoute ToEventAttendeeRoute(this DisplayEventAttendeeRoute displayEventAttendeeRoute)
        {
            var coordinates = new List<Coordinate>();

            foreach (var location in displayEventAttendeeRoute.Locations.OrderBy(x => x.SortOrder))
            {
                coordinates.Add(new Coordinate(location.Longitude, location.Latitude));
            }

            var userPath = new LineString([.. coordinates]);

            return new EventAttendeeRoute
            {
                Id = displayEventAttendeeRoute.Id,
                EventId = displayEventAttendeeRoute.EventId,
                UserId = displayEventAttendeeRoute.UserId,
                EndTime = displayEventAttendeeRoute.EndTime,
                StartTime = displayEventAttendeeRoute.StartTime,
                UserPath = userPath,
            };
        }
    }
}
