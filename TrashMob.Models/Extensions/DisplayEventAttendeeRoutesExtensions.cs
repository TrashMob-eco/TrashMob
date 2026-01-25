namespace TrashMob.Models.Extensions
{
    using System.Collections.Generic;
    using NetTopologySuite.Geometries;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Provides extension methods for converting DisplayEventAttendeeRoute to EventAttendeeRoute.
    /// </summary>
    public static class DisplayEventAttendeeRoutesExtensions
    {
        /// <summary>
        /// Converts a DisplayEventAttendeeRoute to an EventAttendeeRoute with geometric path data.
        /// </summary>
        /// <param name="displayEventAttendeeRoute">The display route to convert.</param>
        /// <returns>An EventAttendeeRoute with the path represented as a LineString geometry.</returns>
        public static EventAttendeeRoute ToEventAttendeeRoute(this DisplayEventAttendeeRoute displayEventAttendeeRoute)
        {
            var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var coordinates = new List<Coordinate>();

            foreach (var location in displayEventAttendeeRoute.Locations.OrderBy(x => x.SortOrder))
            {
                coordinates.Add(new Coordinate(location.Longitude, location.Latitude));
            }

            var userPath = geometryFactory.CreateLineString([.. coordinates]);

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
