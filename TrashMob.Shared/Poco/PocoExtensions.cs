namespace TrashMob.Shared.Poco
{
    using System.Linq;
    using TrashMob.Models;

    public static class PocoExtensions
    {
        public static DisplayUser ToDisplayUser(this User user)
        {
            return new DisplayUser
            {
                City = user.City,
                Country = user.Country,
                UserName = user.UserName,
                Email = user.Email,
                Id = user.Id,
                MemberSince = user.MemberSince,
                Region = user.Region
            };
        }

        public static DisplayEvent ToDisplayEvent(this Event mobEvent, string userName)
        {
            return new DisplayEvent
            {
                Id = mobEvent.Id,
                Name = mobEvent.Name,
                Description = mobEvent.Description,
                EventDate = mobEvent.EventDate,
                DurationHours = mobEvent.DurationHours,
                DurationMinutes = mobEvent.DurationMinutes,
                EventTypeId = mobEvent.EventTypeId,
                EventStatusId = mobEvent.EventStatusId,
                StreetAddress = mobEvent.StreetAddress,
                City = mobEvent.City,
                Region = mobEvent.Region,
                Country = mobEvent.Country,
                PostalCode = mobEvent.PostalCode,
                Latitude = mobEvent.Latitude,
                Longitude = mobEvent.Longitude,
                MaxNumberOfParticipants = mobEvent.MaxNumberOfParticipants,
                IsEventPublic = mobEvent.IsEventPublic,
                CreatedByUserId = mobEvent.CreatedByUserId,
                CreatedDate = mobEvent.CreatedDate,
                LastUpdatedByUserId = mobEvent.LastUpdatedByUserId,
                LastUpdatedDate = mobEvent.LastUpdatedDate,
                CreatedByUserName = userName,
            };
        }


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
                LitterImages = litterReport.LitterImages == null ? null : litterReport.LitterImages.Select(image => image.ToFullLitterImage()).ToList(),
                CreateByUserName = userName,
            };
        }

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
            };
        }

        public static FullLitterImage ToFullLitterImage(this LitterImage litterImage)
        {
            return new FullLitterImage
            {
                Id = litterImage.Id,
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
