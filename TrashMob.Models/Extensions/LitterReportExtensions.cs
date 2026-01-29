namespace TrashMob.Models.Extensions
{
    using TrashMob.Models.Poco;

    /// <summary>
    /// Provides extension methods for converting between LitterReport and FullLitterReport types.
    /// </summary>
    public static class LitterReportExtensions
    {
        /// <summary>
        /// Converts a LitterReport to a FullLitterReport including the creator's user name.
        /// </summary>
        /// <param name="litterReport">The litter report to convert.</param>
        /// <param name="userName">The name of the user who created the report.</param>
        /// <returns>A FullLitterReport with all properties including user name and full litter images.</returns>
        public static FullLitterReport ToFullLitterReport(this LitterReport litterReport, string userName)
        {
            return new FullLitterReport
            {
                Id = litterReport.Id,
                Name = litterReport.Name,
                Description = litterReport.Description,
                LitterReportStatusId = litterReport.LitterReportStatusId,
                CreatedByUserId = litterReport.CreatedByUserId,
                CreatedDate = litterReport.CreatedDate,
                LastUpdatedByUserId = litterReport.LastUpdatedByUserId,
                LastUpdatedDate = litterReport.LastUpdatedDate,
                LitterImages = litterReport.LitterImages == null
                    ? []
                    : litterReport.LitterImages.Select(image => image.ToFullLitterImage()).ToList(),
                CreateByUserName = userName,
            };
        }

        /// <summary>
        /// Converts a LitterReport to a FullLitterReport without user name information.
        /// </summary>
        /// <param name="litterReport">The litter report to convert.</param>
        /// <returns>A FullLitterReport with all properties and full litter images.</returns>
        public static FullLitterReport ToFullLitterReport(this LitterReport litterReport)
        {
            return new FullLitterReport
            {
                Id = litterReport.Id,
                Name = litterReport.Name,
                Description = litterReport.Description,
                LitterReportStatusId = litterReport.LitterReportStatusId,
                CreatedByUserId = litterReport.CreatedByUserId,
                CreatedDate = litterReport.CreatedDate,
                LastUpdatedByUserId = litterReport.LastUpdatedByUserId,
                LastUpdatedDate = litterReport.LastUpdatedDate,
                LitterImages = litterReport.LitterImages == null
                    ? []
                    : litterReport.LitterImages.Select(image => image.ToFullLitterImage()).ToList(),
            };
        }

        /// <summary>
        /// Converts a FullLitterReport back to a LitterReport entity.
        /// </summary>
        /// <param name="fullLitterReport">The full litter report to convert.</param>
        /// <returns>A LitterReport entity with associated litter images.</returns>
        public static LitterReport ToLitterReport(this FullLitterReport fullLitterReport)
        {
            return new LitterReport
            {
                Id = fullLitterReport.Id,
                Name = fullLitterReport.Name,
                Description = fullLitterReport.Description,
                LitterReportStatusId = fullLitterReport.LitterReportStatusId,
                CreatedByUserId = fullLitterReport.CreatedByUserId,
                CreatedDate = fullLitterReport.CreatedDate,
                LastUpdatedByUserId = fullLitterReport.LastUpdatedByUserId,
                LastUpdatedDate = fullLitterReport.LastUpdatedDate,
                LitterImages = fullLitterReport.LitterImages.Select(x => x.ToLitterImage()).ToList()
            };
        }

        /// <summary>
        /// Converts a LitterImage to a FullLitterImage with the image URL exposed.
        /// </summary>
        /// <param name="litterImage">The litter image to convert.</param>
        /// <returns>A FullLitterImage with the Azure blob URL mapped to ImageURL.</returns>
        public static FullLitterImage ToFullLitterImage(this LitterImage litterImage)
        {
            return new FullLitterImage
            {
                Id = litterImage.Id == Guid.Empty ? Guid.NewGuid() : litterImage.Id,
                LitterReportId = litterImage.LitterReportId,
                ImageURL = litterImage.AzureBlobURL,
                StreetAddress = litterImage.StreetAddress,
                City = litterImage.City,
                PostalCode = litterImage.PostalCode,
                Country = litterImage.Country,
                Region = litterImage.Region,
                Latitude = litterImage.Latitude,
                Longitude = litterImage.Longitude,
                CreatedByUserId = litterImage.CreatedByUserId,
                CreatedDate = litterImage.CreatedDate,
                LastUpdatedByUserId = litterImage.LastUpdatedByUserId,
                LastUpdatedDate = litterImage.LastUpdatedDate,
            };
        }

        /// <summary>
        /// Converts a FullLitterImage back to a LitterImage entity.
        /// </summary>
        /// <param name="fullLitterImage">The full litter image to convert.</param>
        /// <returns>A LitterImage entity with the ImageURL mapped to AzureBlobURL.</returns>
        public static LitterImage ToLitterImage(this FullLitterImage fullLitterImage)
        {
            return new LitterImage
            {
                Id = fullLitterImage.Id,
                LitterReportId = fullLitterImage.LitterReportId,
                AzureBlobURL = fullLitterImage.ImageURL,
                StreetAddress = fullLitterImage.StreetAddress,
                City = fullLitterImage.City,
                PostalCode = fullLitterImage.PostalCode,
                Country = fullLitterImage.Country,
                Region = fullLitterImage.Region,
                Latitude = fullLitterImage.Latitude,
                Longitude = fullLitterImage.Longitude,
                CreatedByUserId = fullLitterImage.CreatedByUserId,
                CreatedDate = fullLitterImage.CreatedDate,
                LastUpdatedByUserId = fullLitterImage.LastUpdatedByUserId,
                LastUpdatedDate = fullLitterImage.LastUpdatedDate,
            };
        }
    }
}