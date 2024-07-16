namespace TrashMob.Models.Poco
{
    public class DisplayEventAttendeeRoute
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public List<SortableLocation> Locations { get; set; } = [];
    }
}