#nullable disable

namespace TrashMob.Models
{
    using NetTopologySuite.Geometries;
    using System;

    public partial class EventAttendeeRoute : KeyedModel
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public LineString UserPath { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public virtual Event Event { get; set; }

        public virtual User User { get; set; }
    }
}
