namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    public static class PartnerManagementMappingsV2
    {
        public static PartnerLocationDto ToV2Dto(this PartnerLocation entity) => new()
        {
            Id = entity.Id,
            PartnerId = entity.PartnerId,
            Name = entity.Name,
            StreetAddress = entity.StreetAddress,
            City = entity.City,
            Region = entity.Region,
            Country = entity.Country,
            PostalCode = entity.PostalCode,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            PublicNotes = entity.PublicNotes,
            PrivateNotes = entity.PrivateNotes,
            IsActive = entity.IsActive,
            CreatedDate = entity.CreatedDate,
        };

        public static PartnerLocation ToEntity(this PartnerLocationDto dto) => new()
        {
            Id = dto.Id,
            PartnerId = dto.PartnerId,
            Name = dto.Name,
            StreetAddress = dto.StreetAddress,
            City = dto.City,
            Region = dto.Region,
            Country = dto.Country,
            PostalCode = dto.PostalCode,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            PublicNotes = dto.PublicNotes,
            PrivateNotes = dto.PrivateNotes,
            IsActive = dto.IsActive,
        };

        public static PartnerContactDto ToV2Dto(this PartnerContact entity) => new()
        {
            Id = entity.Id,
            PartnerId = entity.PartnerId,
            Name = entity.Name,
            Email = entity.Email,
            Phone = entity.Phone,
            Notes = entity.Notes,
            IsActive = entity.IsActive,
            CreatedDate = entity.CreatedDate,
        };

        public static PartnerContact ToEntity(this PartnerContactDto dto) => new()
        {
            Id = dto.Id,
            PartnerId = dto.PartnerId,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Notes = dto.Notes,
            IsActive = dto.IsActive,
        };

        public static PartnerLocationContactDto ToV2Dto(this PartnerLocationContact entity) => new()
        {
            Id = entity.Id,
            PartnerLocationId = entity.PartnerLocationId,
            Name = entity.Name,
            Email = entity.Email,
            Phone = entity.Phone,
            Notes = entity.Notes,
            IsActive = entity.IsActive,
            CreatedDate = entity.CreatedDate,
        };

        public static PartnerLocationContact ToEntity(this PartnerLocationContactDto dto) => new()
        {
            Id = dto.Id,
            PartnerLocationId = dto.PartnerLocationId,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Notes = dto.Notes,
            IsActive = dto.IsActive,
        };

        public static PartnerLocationServiceDto ToV2Dto(this PartnerLocationService entity) => new()
        {
            PartnerLocationId = entity.PartnerLocationId,
            ServiceTypeId = entity.ServiceTypeId,
            IsAutoApproved = entity.IsAutoApproved,
            IsAdvanceNoticeRequired = entity.IsAdvanceNoticeRequired,
            Notes = entity.Notes,
            CreatedDate = entity.CreatedDate,
        };

        public static PartnerLocationService ToEntity(this PartnerLocationServiceDto dto) => new()
        {
            PartnerLocationId = dto.PartnerLocationId,
            ServiceTypeId = dto.ServiceTypeId,
            IsAutoApproved = dto.IsAutoApproved,
            IsAdvanceNoticeRequired = dto.IsAdvanceNoticeRequired,
            Notes = dto.Notes,
        };

        public static PartnerAdminDto ToV2Dto(this PartnerAdmin entity) => new()
        {
            PartnerId = entity.PartnerId,
            UserId = entity.UserId,
            CreatedDate = entity.CreatedDate,
        };

        public static PartnerAdminInvitationDto ToV2Dto(this PartnerAdminInvitation entity) => new()
        {
            Id = entity.Id,
            PartnerId = entity.PartnerId,
            Email = entity.Email,
            InvitationStatusId = entity.InvitationStatusId,
            DateInvited = entity.DateInvited,
            CreatedDate = entity.CreatedDate,
        };

        public static PartnerAdminInvitation ToEntity(this PartnerAdminInvitationDto dto) => new()
        {
            Id = dto.Id,
            PartnerId = dto.PartnerId,
            Email = dto.Email,
            InvitationStatusId = dto.InvitationStatusId,
            DateInvited = dto.DateInvited,
        };

        public static PartnerRequestDto ToV2Dto(this PartnerRequest entity) => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Website = entity.Website,
            Phone = entity.Phone,
            City = entity.City,
            Region = entity.Region,
            Country = entity.Country,
            PostalCode = entity.PostalCode,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            Notes = entity.Notes,
            PartnerRequestStatusId = entity.PartnerRequestStatusId,
            PartnerTypeId = entity.PartnerTypeId,
            IsBecomeAPartnerRequest = entity.isBecomeAPartnerRequest,
            CreatedDate = entity.CreatedDate,
        };

        public static PartnerRequest ToEntity(this PartnerRequestDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            Website = dto.Website,
            Phone = dto.Phone,
            City = dto.City,
            Region = dto.Region,
            Country = dto.Country,
            PostalCode = dto.PostalCode,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Notes = dto.Notes,
            PartnerRequestStatusId = dto.PartnerRequestStatusId,
            PartnerTypeId = dto.PartnerTypeId,
            isBecomeAPartnerRequest = dto.IsBecomeAPartnerRequest,
        };

        public static PartnerDocumentDto ToV2Dto(this PartnerDocument entity) => new()
        {
            Id = entity.Id,
            PartnerId = entity.PartnerId,
            Name = entity.Name,
            Url = entity.Url,
            ContentType = entity.ContentType,
            FileSizeBytes = entity.FileSizeBytes,
            DocumentTypeId = entity.DocumentTypeId,
            ExpirationDate = entity.ExpirationDate,
            CreatedDate = entity.CreatedDate,
        };

        public static PartnerDocument ToEntity(this PartnerDocumentDto dto) => new()
        {
            Id = dto.Id,
            PartnerId = dto.PartnerId,
            Name = dto.Name,
            Url = dto.Url,
            ContentType = dto.ContentType,
            FileSizeBytes = dto.FileSizeBytes,
            DocumentTypeId = dto.DocumentTypeId,
            ExpirationDate = dto.ExpirationDate,
        };

        public static PartnerSocialMediaAccountDto ToV2Dto(this PartnerSocialMediaAccount entity) => new()
        {
            Id = entity.Id,
            PartnerId = entity.PartnerId,
            AccountIdentifier = entity.AccountIdentifier,
            IsActive = entity.IsActive,
            SocialMediaAccountTypeId = entity.SocialMediaAccountTypeId,
            CreatedDate = entity.CreatedDate,
        };

        public static PartnerSocialMediaAccount ToEntity(this PartnerSocialMediaAccountDto dto) => new()
        {
            Id = dto.Id,
            PartnerId = dto.PartnerId,
            AccountIdentifier = dto.AccountIdentifier,
            IsActive = dto.IsActive,
            SocialMediaAccountTypeId = dto.SocialMediaAccountTypeId,
        };
    }
}
