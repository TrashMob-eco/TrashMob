namespace TrashMob.Models.Poco
{
    public class FullEventLitterReport
    {
        public Guid EventId { get; set; }

        public Guid LitterReportId { get; set; }

        public required FullLitterReport LitterReport { get; set; }
    }
}