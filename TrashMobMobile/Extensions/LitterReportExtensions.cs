namespace TrashMobMobile.Extensions
{
    using TrashMob.Models;

    public static class LitterReportExtensions
    {
        public static string GetLitterStatusFromId(int? id)
        {
            return id == null ? string.Empty : Enum.GetName(typeof(LitterReportStatusEnum), id.Value) ?? string.Empty;
        }

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

        public static LitterReport ToLitterReport(this LitterReportViewModel litterReportViewModel)
        {
            return new LitterReport
            {
                Id = litterReportViewModel.Id,
                Name = litterReportViewModel.Name,
                Description = litterReportViewModel.Description,
                LitterReportStatusId = litterReportViewModel.LitterReportStatusId,
                LitterImages = [],
            };
        }

        public static LitterImage ToLitterImage(this LitterImageViewModel litterImageViewModel)
        {
            return new LitterImage
            {
                Id = litterImageViewModel.Id,
                LitterReportId = litterImageViewModel.LitterReportId,
            };
        }

        public static LitterImageViewModel ToLitterImageViewModel(this LitterImage litterImage)
        {
            return new LitterImageViewModel
            {
                Id = litterImage.Id,
                LitterReportId = litterImage.LitterReportId,
                AzureBlobUrl = litterImage.AzureBlobURL,
                Address = new AddressViewModel
                {
                    City = litterImage.City,
                    Country = litterImage.Country,
                    Latitude = litterImage.Latitude,
                    Longitude = litterImage.Longitude,
                    PostalCode = litterImage.PostalCode,
                    Region = litterImage.Region,
                    StreetAddress = litterImage.StreetAddress,
                    Location = new Location(litterImage.Latitude.Value, litterImage.Longitude.Value)
                },
            };
        }
    }
}
