namespace TrashMobMobile.Extensions
{
    using TrashMob.Models;

    public static class LitterReportExtensions
    {
        public static LitterReportViewModel ToLitterReportViewModel(this LitterReport litterReport)
        {
            return new LitterReportViewModel
            {
                Id = litterReport.Id,
                Name = litterReport.Name,
                Description = litterReport.Description,
                LitterReportStatusId = litterReport.LitterReportStatusId,
            };
        }
    }
}
