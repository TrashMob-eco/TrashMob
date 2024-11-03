namespace TrashMob.Models.Poco
{
    public class LitterReportFilter : GeneralFilter
    {
        public int? LitterReportStatusId { get; set; }

        public bool IncludeLitterImages { get; set; } = false;
    }
}