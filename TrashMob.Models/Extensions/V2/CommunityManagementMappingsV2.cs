namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping community management entities to V2 DTOs.
    /// </summary>
    public static class CommunityManagementMappingsV2
    {
        #region AdoptableArea

        /// <summary>
        /// Maps an AdoptableArea entity to a V2 AdoptableAreaDto.
        /// </summary>
        public static AdoptableAreaDto ToV2Dto(this AdoptableArea entity)
        {
            return new AdoptableAreaDto
            {
                Id = entity.Id,
                PartnerId = entity.PartnerId,
                Name = entity.Name,
                Description = entity.Description,
                AreaType = entity.AreaType,
                Status = entity.Status,
                GeoJson = entity.GeoJson,
                StartLatitude = entity.StartLatitude,
                StartLongitude = entity.StartLongitude,
                EndLatitude = entity.EndLatitude,
                EndLongitude = entity.EndLongitude,
                CleanupFrequencyDays = entity.CleanupFrequencyDays,
                MinEventsPerYear = entity.MinEventsPerYear,
                SafetyRequirements = entity.SafetyRequirements,
                AllowCoAdoption = entity.AllowCoAdoption,
                IsActive = entity.IsActive,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate,
                LastUpdatedByUserId = entity.LastUpdatedByUserId,
                LastUpdatedDate = entity.LastUpdatedDate,
            };
        }

        /// <summary>
        /// Maps a V2 AdoptableAreaDto to an AdoptableArea entity.
        /// </summary>
        public static AdoptableArea ToEntity(this AdoptableAreaDto dto)
        {
            return new AdoptableArea
            {
                Id = dto.Id,
                PartnerId = dto.PartnerId,
                Name = dto.Name ?? string.Empty,
                Description = dto.Description,
                AreaType = dto.AreaType,
                Status = dto.Status ?? "Available",
                GeoJson = dto.GeoJson,
                StartLatitude = dto.StartLatitude,
                StartLongitude = dto.StartLongitude,
                EndLatitude = dto.EndLatitude,
                EndLongitude = dto.EndLongitude,
                CleanupFrequencyDays = dto.CleanupFrequencyDays,
                MinEventsPerYear = dto.MinEventsPerYear,
                SafetyRequirements = dto.SafetyRequirements,
                AllowCoAdoption = dto.AllowCoAdoption,
                IsActive = dto.IsActive,
                CreatedByUserId = dto.CreatedByUserId,
                CreatedDate = dto.CreatedDate,
                LastUpdatedByUserId = dto.LastUpdatedByUserId,
                LastUpdatedDate = dto.LastUpdatedDate,
            };
        }

        #endregion

        #region StagedAdoptableArea

        /// <summary>
        /// Maps a StagedAdoptableArea entity to a V2 StagedAdoptableAreaDto.
        /// </summary>
        public static StagedAdoptableAreaDto ToV2Dto(this StagedAdoptableArea entity)
        {
            return new StagedAdoptableAreaDto
            {
                Id = entity.Id,
                BatchId = entity.BatchId,
                PartnerId = entity.PartnerId,
                Name = entity.Name,
                Description = entity.Description,
                AreaType = entity.AreaType,
                GeoJson = entity.GeoJson,
                CenterLatitude = entity.CenterLatitude,
                CenterLongitude = entity.CenterLongitude,
                ReviewStatus = entity.ReviewStatus,
                Confidence = entity.Confidence,
                IsPotentialDuplicate = entity.IsPotentialDuplicate,
                DuplicateOfName = entity.DuplicateOfName,
                OsmId = entity.OsmId,
                OsmTags = entity.OsmTags,
                CreatedDate = entity.CreatedDate,
            };
        }

        /// <summary>
        /// Maps a V2 StagedAdoptableAreaDto to a StagedAdoptableArea entity.
        /// </summary>
        public static StagedAdoptableArea ToEntity(this StagedAdoptableAreaDto dto)
        {
            return new StagedAdoptableArea
            {
                Id = dto.Id,
                BatchId = dto.BatchId,
                PartnerId = dto.PartnerId,
                Name = dto.Name ?? string.Empty,
                Description = dto.Description,
                AreaType = dto.AreaType,
                GeoJson = dto.GeoJson,
                CenterLatitude = dto.CenterLatitude,
                CenterLongitude = dto.CenterLongitude,
                ReviewStatus = dto.ReviewStatus ?? "Pending",
                Confidence = dto.Confidence ?? "Medium",
                IsPotentialDuplicate = dto.IsPotentialDuplicate,
                DuplicateOfName = dto.DuplicateOfName,
                OsmId = dto.OsmId,
                OsmTags = dto.OsmTags,
            };
        }

        #endregion

        #region AreaGenerationBatch

        /// <summary>
        /// Maps an AreaGenerationBatch entity to a V2 AreaGenerationBatchDto.
        /// </summary>
        public static AreaGenerationBatchDto ToV2Dto(this AreaGenerationBatch entity)
        {
            return new AreaGenerationBatchDto
            {
                Id = entity.Id,
                PartnerId = entity.PartnerId,
                Category = entity.Category,
                Status = entity.Status,
                DiscoveredCount = entity.DiscoveredCount,
                ProcessedCount = entity.ProcessedCount,
                SkippedCount = entity.SkippedCount,
                StagedCount = entity.StagedCount,
                ApprovedCount = entity.ApprovedCount,
                RejectedCount = entity.RejectedCount,
                CreatedCount = entity.CreatedCount,
                ErrorMessage = entity.ErrorMessage,
                CompletedDate = entity.CompletedDate,
                BoundsNorth = entity.BoundsNorth,
                BoundsSouth = entity.BoundsSouth,
                BoundsEast = entity.BoundsEast,
                BoundsWest = entity.BoundsWest,
                CreatedDate = entity.CreatedDate,
            };
        }

        /// <summary>
        /// Maps a V2 AreaGenerationBatchDto to an AreaGenerationBatch entity.
        /// </summary>
        public static AreaGenerationBatch ToEntity(this AreaGenerationBatchDto dto)
        {
            return new AreaGenerationBatch
            {
                Id = dto.Id,
                PartnerId = dto.PartnerId,
                Category = dto.Category,
                Status = dto.Status ?? "Queued",
                DiscoveredCount = dto.DiscoveredCount,
                ProcessedCount = dto.ProcessedCount,
                SkippedCount = dto.SkippedCount,
                StagedCount = dto.StagedCount,
                ApprovedCount = dto.ApprovedCount,
                RejectedCount = dto.RejectedCount,
                CreatedCount = dto.CreatedCount,
                ErrorMessage = dto.ErrorMessage,
                CompletedDate = dto.CompletedDate,
                BoundsNorth = dto.BoundsNorth,
                BoundsSouth = dto.BoundsSouth,
                BoundsEast = dto.BoundsEast,
                BoundsWest = dto.BoundsWest,
            };
        }

        #endregion

        #region TeamAdoption

        /// <summary>
        /// Maps a TeamAdoption entity to a V2 TeamAdoptionDto.
        /// </summary>
        public static TeamAdoptionDto ToV2Dto(this TeamAdoption entity)
        {
            return new TeamAdoptionDto
            {
                Id = entity.Id,
                TeamId = entity.TeamId,
                AdoptableAreaId = entity.AdoptableAreaId,
                ApplicationDate = entity.ApplicationDate,
                ApplicationNotes = entity.ApplicationNotes,
                Status = entity.Status,
                ReviewedByUserId = entity.ReviewedByUserId,
                ReviewedDate = entity.ReviewedDate,
                RejectionReason = entity.RejectionReason,
                AdoptionStartDate = entity.AdoptionStartDate,
                AdoptionEndDate = entity.AdoptionEndDate,
                LastEventDate = entity.LastEventDate,
                EventCount = entity.EventCount,
                IsCompliant = entity.IsCompliant,
                CreatedDate = entity.CreatedDate,
            };
        }

        /// <summary>
        /// Maps a V2 TeamAdoptionDto to a TeamAdoption entity.
        /// </summary>
        public static TeamAdoption ToEntity(this TeamAdoptionDto dto)
        {
            return new TeamAdoption
            {
                Id = dto.Id,
                TeamId = dto.TeamId,
                AdoptableAreaId = dto.AdoptableAreaId,
                ApplicationDate = dto.ApplicationDate,
                ApplicationNotes = dto.ApplicationNotes,
                Status = dto.Status ?? "Pending",
                ReviewedByUserId = dto.ReviewedByUserId,
                ReviewedDate = dto.ReviewedDate,
                RejectionReason = dto.RejectionReason,
                AdoptionStartDate = dto.AdoptionStartDate,
                AdoptionEndDate = dto.AdoptionEndDate,
                LastEventDate = dto.LastEventDate,
                EventCount = dto.EventCount,
                IsCompliant = dto.IsCompliant,
            };
        }

        #endregion

        #region TeamAdoptionEvent

        /// <summary>
        /// Maps a TeamAdoptionEvent entity to a V2 TeamAdoptionEventDto.
        /// </summary>
        public static TeamAdoptionEventDto ToV2Dto(this TeamAdoptionEvent entity)
        {
            return new TeamAdoptionEventDto
            {
                Id = entity.Id,
                TeamAdoptionId = entity.TeamAdoptionId,
                EventId = entity.EventId,
                Notes = entity.Notes,
                CreatedDate = entity.CreatedDate,
            };
        }

        /// <summary>
        /// Maps a V2 TeamAdoptionEventDto to a TeamAdoptionEvent entity.
        /// </summary>
        public static TeamAdoptionEvent ToEntity(this TeamAdoptionEventDto dto)
        {
            return new TeamAdoptionEvent
            {
                Id = dto.Id,
                TeamAdoptionId = dto.TeamAdoptionId,
                EventId = dto.EventId,
                Notes = dto.Notes,
            };
        }

        #endregion

        #region SponsoredAdoption

        /// <summary>
        /// Maps a SponsoredAdoption entity to a V2 SponsoredAdoptionDto.
        /// </summary>
        public static SponsoredAdoptionDto ToV2Dto(this SponsoredAdoption entity)
        {
            return new SponsoredAdoptionDto
            {
                Id = entity.Id,
                AdoptableAreaId = entity.AdoptableAreaId,
                SponsorId = entity.SponsorId,
                ProfessionalCompanyId = entity.ProfessionalCompanyId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                CleanupFrequencyDays = entity.CleanupFrequencyDays,
                Status = entity.Status,
                CreatedDate = entity.CreatedDate,
            };
        }

        /// <summary>
        /// Maps a V2 SponsoredAdoptionDto to a SponsoredAdoption entity.
        /// </summary>
        public static SponsoredAdoption ToEntity(this SponsoredAdoptionDto dto)
        {
            return new SponsoredAdoption
            {
                Id = dto.Id,
                AdoptableAreaId = dto.AdoptableAreaId,
                SponsorId = dto.SponsorId,
                ProfessionalCompanyId = dto.ProfessionalCompanyId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CleanupFrequencyDays = dto.CleanupFrequencyDays,
                Status = dto.Status ?? "Active",
            };
        }

        #endregion

        #region Sponsor

        /// <summary>
        /// Maps a Sponsor entity to a V2 SponsorDto.
        /// </summary>
        public static SponsorDto ToV2Dto(this Sponsor entity)
        {
            return new SponsorDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ContactEmail = entity.ContactEmail,
                ContactPhone = entity.ContactPhone,
                LogoUrl = entity.LogoUrl,
                PartnerId = entity.PartnerId,
                IsActive = entity.IsActive,
                ShowOnPublicMap = entity.ShowOnPublicMap,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate,
                LastUpdatedByUserId = entity.LastUpdatedByUserId,
                LastUpdatedDate = entity.LastUpdatedDate,
            };
        }

        /// <summary>
        /// Maps a V2 SponsorDto to a Sponsor entity.
        /// </summary>
        public static Sponsor ToEntity(this SponsorDto dto)
        {
            return new Sponsor
            {
                Id = dto.Id,
                Name = dto.Name ?? string.Empty,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                LogoUrl = dto.LogoUrl,
                PartnerId = dto.PartnerId,
                IsActive = dto.IsActive,
                ShowOnPublicMap = dto.ShowOnPublicMap,
                CreatedByUserId = dto.CreatedByUserId,
                CreatedDate = dto.CreatedDate,
                LastUpdatedByUserId = dto.LastUpdatedByUserId,
                LastUpdatedDate = dto.LastUpdatedDate,
            };
        }

        #endregion

        #region ProfessionalCompany

        /// <summary>
        /// Maps a ProfessionalCompany entity to a V2 ProfessionalCompanyDto.
        /// </summary>
        public static ProfessionalCompanyDto ToV2Dto(this ProfessionalCompany entity)
        {
            return new ProfessionalCompanyDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ContactEmail = entity.ContactEmail,
                ContactPhone = entity.ContactPhone,
                PartnerId = entity.PartnerId,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
            };
        }

        /// <summary>
        /// Maps a V2 ProfessionalCompanyDto to a ProfessionalCompany entity.
        /// </summary>
        public static ProfessionalCompany ToEntity(this ProfessionalCompanyDto dto)
        {
            return new ProfessionalCompany
            {
                Id = dto.Id,
                Name = dto.Name ?? string.Empty,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                PartnerId = dto.PartnerId,
                IsActive = dto.IsActive,
            };
        }

        #endregion

        #region ProfessionalCompanyUser

        /// <summary>
        /// Maps a ProfessionalCompanyUser entity to a V2 ProfessionalCompanyUserDto.
        /// </summary>
        public static ProfessionalCompanyUserDto ToV2Dto(this ProfessionalCompanyUser entity)
        {
            return new ProfessionalCompanyUserDto
            {
                ProfessionalCompanyId = entity.ProfessionalCompanyId,
                UserId = entity.UserId,
                CreatedDate = entity.CreatedDate,
            };
        }

        /// <summary>
        /// Maps a V2 ProfessionalCompanyUserDto to a ProfessionalCompanyUser entity.
        /// </summary>
        public static ProfessionalCompanyUser ToEntity(this ProfessionalCompanyUserDto dto)
        {
            return new ProfessionalCompanyUser
            {
                ProfessionalCompanyId = dto.ProfessionalCompanyId,
                UserId = dto.UserId,
            };
        }

        #endregion

        #region CommunityProspect

        /// <summary>
        /// Maps a CommunityProspect entity to a V2 CommunityProspectDto.
        /// </summary>
        public static CommunityProspectDto ToV2Dto(this CommunityProspect entity)
        {
            return new CommunityProspectDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Type = entity.Type,
                City = entity.City,
                Region = entity.Region,
                Country = entity.Country,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Population = entity.Population,
                Website = entity.Website,
                ContactEmail = entity.ContactEmail,
                ContactName = entity.ContactName,
                ContactTitle = entity.ContactTitle,
                ContactPhone = entity.ContactPhone,
                PipelineStage = entity.PipelineStage,
                FitScore = entity.FitScore,
                Notes = entity.Notes,
                LastContactedDate = entity.LastContactedDate,
                NextFollowUpDate = entity.NextFollowUpDate,
                ConvertedPartnerId = entity.ConvertedPartnerId,
                CreatedDate = entity.CreatedDate,
            };
        }

        /// <summary>
        /// Maps a V2 CommunityProspectDto to a CommunityProspect entity.
        /// </summary>
        public static CommunityProspect ToEntity(this CommunityProspectDto dto)
        {
            return new CommunityProspect
            {
                Id = dto.Id,
                Name = dto.Name ?? string.Empty,
                Type = dto.Type,
                City = dto.City,
                Region = dto.Region,
                Country = dto.Country,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Population = dto.Population,
                Website = dto.Website,
                ContactEmail = dto.ContactEmail,
                ContactName = dto.ContactName,
                ContactTitle = dto.ContactTitle,
                ContactPhone = dto.ContactPhone,
                PipelineStage = dto.PipelineStage,
                FitScore = dto.FitScore,
                Notes = dto.Notes,
                LastContactedDate = dto.LastContactedDate,
                NextFollowUpDate = dto.NextFollowUpDate,
                ConvertedPartnerId = dto.ConvertedPartnerId,
            };
        }

        #endregion

        #region ProspectActivity

        /// <summary>
        /// Maps a ProspectActivity entity to a V2 ProspectActivityDto.
        /// </summary>
        public static ProspectActivityDto ToV2Dto(this ProspectActivity entity)
        {
            return new ProspectActivityDto
            {
                Id = entity.Id,
                ProspectId = entity.ProspectId,
                ActivityType = entity.ActivityType,
                Subject = entity.Subject,
                Details = entity.Details,
                SentimentScore = entity.SentimentScore,
                CreatedDate = entity.CreatedDate,
            };
        }

        /// <summary>
        /// Maps a V2 ProspectActivityDto to a ProspectActivity entity.
        /// </summary>
        public static ProspectActivity ToEntity(this ProspectActivityDto dto)
        {
            return new ProspectActivity
            {
                Id = dto.Id,
                ProspectId = dto.ProspectId,
                ActivityType = dto.ActivityType ?? string.Empty,
                Subject = dto.Subject,
                Details = dto.Details,
                SentimentScore = dto.SentimentScore,
            };
        }

        #endregion
    }
}
