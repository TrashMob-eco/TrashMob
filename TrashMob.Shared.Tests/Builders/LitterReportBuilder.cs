namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using System.Collections.Generic;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating LitterReport test data with sensible defaults.
    /// </summary>
    public class LitterReportBuilder
    {
        private readonly LitterReport _litterReport;

        public LitterReportBuilder()
        {
            var creatorId = Guid.NewGuid();
            _litterReport = new LitterReport
            {
                Id = Guid.NewGuid(),
                Name = "Test Litter Report",
                Description = "A test litter report for unit testing",
                LitterReportStatusId = (int)LitterReportStatusEnum.New,
                LitterImages = new List<LitterImage>(),
                CreatedByUserId = creatorId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = creatorId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };
        }

        public LitterReportBuilder WithId(Guid id)
        {
            _litterReport.Id = id;
            return this;
        }

        public LitterReportBuilder WithName(string name)
        {
            _litterReport.Name = name;
            return this;
        }

        public LitterReportBuilder WithDescription(string description)
        {
            _litterReport.Description = description;
            return this;
        }

        public LitterReportBuilder WithStatus(LitterReportStatusEnum status)
        {
            _litterReport.LitterReportStatusId = (int)status;
            return this;
        }

        public LitterReportBuilder AsNew()
        {
            _litterReport.LitterReportStatusId = (int)LitterReportStatusEnum.New;
            return this;
        }

        public LitterReportBuilder AsAssigned()
        {
            _litterReport.LitterReportStatusId = (int)LitterReportStatusEnum.Assigned;
            return this;
        }

        public LitterReportBuilder AsCleaned()
        {
            _litterReport.LitterReportStatusId = (int)LitterReportStatusEnum.Cleaned;
            return this;
        }

        public LitterReportBuilder AsCancelled()
        {
            _litterReport.LitterReportStatusId = (int)LitterReportStatusEnum.Cancelled;
            return this;
        }

        public LitterReportBuilder WithImage(LitterImage image)
        {
            _litterReport.LitterImages.Add(image);
            return this;
        }

        public LitterReportBuilder WithImages(IEnumerable<LitterImage> images)
        {
            foreach (var image in images)
            {
                _litterReport.LitterImages.Add(image);
            }
            return this;
        }

        public LitterReportBuilder WithDefaultImage()
        {
            _litterReport.LitterImages.Add(new LitterImage
            {
                Id = Guid.NewGuid(),
                LitterReportId = _litterReport.Id,
                AzureBlobURL = "https://example.com/image.jpg",
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                CreatedByUserId = _litterReport.CreatedByUserId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = _litterReport.CreatedByUserId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            });
            return this;
        }

        public LitterReportBuilder CreatedBy(Guid userId)
        {
            _litterReport.CreatedByUserId = userId;
            _litterReport.LastUpdatedByUserId = userId;
            return this;
        }

        public LitterReport Build() => _litterReport;
    }
}
