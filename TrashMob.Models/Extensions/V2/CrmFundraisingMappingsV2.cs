namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    public static class CrmFundraisingMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="Contact"/> entity to a <see cref="ContactDto"/>.
        /// </summary>
        public static ContactDto ToV2Dto(this Contact entity) => new()
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Phone = entity.Phone,
            OrganizationName = entity.OrganizationName,
            Title = entity.Title,
            Address = entity.Address,
            City = entity.City,
            Region = entity.Region,
            PostalCode = entity.PostalCode,
            Country = entity.Country,
            ContactType = entity.ContactType,
            Source = entity.Source,
            UserId = entity.UserId,
            PartnerId = entity.PartnerId,
            Notes = entity.Notes,
            IsActive = entity.IsActive,
            CreatedDate = entity.CreatedDate,
        };

        /// <summary>
        /// Maps a <see cref="ContactDto"/> to a <see cref="Contact"/> entity.
        /// </summary>
        public static Contact ToEntity(this ContactDto dto) => new()
        {
            Id = dto.Id,
            FirstName = dto.FirstName ?? string.Empty,
            LastName = dto.LastName ?? string.Empty,
            Email = dto.Email ?? string.Empty,
            Phone = dto.Phone ?? string.Empty,
            OrganizationName = dto.OrganizationName ?? string.Empty,
            Title = dto.Title ?? string.Empty,
            Address = dto.Address ?? string.Empty,
            City = dto.City ?? string.Empty,
            Region = dto.Region ?? string.Empty,
            PostalCode = dto.PostalCode ?? string.Empty,
            Country = dto.Country ?? string.Empty,
            ContactType = dto.ContactType,
            Source = dto.Source ?? string.Empty,
            UserId = dto.UserId,
            PartnerId = dto.PartnerId,
            Notes = dto.Notes ?? string.Empty,
            IsActive = dto.IsActive,
        };

        /// <summary>
        /// Maps a <see cref="ContactNote"/> entity to a <see cref="ContactNoteDto"/>.
        /// </summary>
        public static ContactNoteDto ToV2Dto(this ContactNote entity) => new()
        {
            Id = entity.Id,
            ContactId = entity.ContactId,
            NoteType = entity.NoteType,
            Subject = entity.Subject,
            Body = entity.Body,
            CreatedDate = entity.CreatedDate,
        };

        /// <summary>
        /// Maps a <see cref="ContactNoteDto"/> to a <see cref="ContactNote"/> entity.
        /// </summary>
        public static ContactNote ToEntity(this ContactNoteDto dto) => new()
        {
            Id = dto.Id,
            ContactId = dto.ContactId,
            NoteType = dto.NoteType,
            Subject = dto.Subject ?? string.Empty,
            Body = dto.Body ?? string.Empty,
        };

        /// <summary>
        /// Maps a <see cref="ContactTag"/> entity to a <see cref="ContactTagDto"/>.
        /// </summary>
        public static ContactTagDto ToV2Dto(this ContactTag entity) => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Color = entity.Color,
            CreatedDate = entity.CreatedDate,
        };

        /// <summary>
        /// Maps a <see cref="ContactTagDto"/> to a <see cref="ContactTag"/> entity.
        /// </summary>
        public static ContactTag ToEntity(this ContactTagDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name ?? string.Empty,
            Color = dto.Color ?? string.Empty,
        };

        /// <summary>
        /// Maps a <see cref="Donation"/> entity to a <see cref="DonationDto"/>.
        /// </summary>
        public static DonationDto ToV2Dto(this Donation entity) => new()
        {
            Id = entity.Id,
            ContactId = entity.ContactId,
            Amount = entity.Amount,
            DonationDate = entity.DonationDate,
            DonationType = entity.DonationType,
            Campaign = entity.Campaign,
            IsRecurring = entity.IsRecurring,
            RecurringFrequency = entity.RecurringFrequency,
            PledgeId = entity.PledgeId,
            InKindDescription = entity.InKindDescription,
            MatchingGiftEmployer = entity.MatchingGiftEmployer,
            Notes = entity.Notes,
            ReceiptSent = entity.ReceiptSent,
            ThankYouSent = entity.ThankYouSent,
            CreatedDate = entity.CreatedDate,
        };

        /// <summary>
        /// Maps a <see cref="DonationDto"/> to a <see cref="Donation"/> entity.
        /// </summary>
        public static Donation ToEntity(this DonationDto dto) => new()
        {
            Id = dto.Id,
            ContactId = dto.ContactId,
            Amount = dto.Amount,
            DonationDate = dto.DonationDate,
            DonationType = dto.DonationType,
            Campaign = dto.Campaign ?? string.Empty,
            IsRecurring = dto.IsRecurring,
            RecurringFrequency = dto.RecurringFrequency,
            PledgeId = dto.PledgeId,
            InKindDescription = dto.InKindDescription ?? string.Empty,
            MatchingGiftEmployer = dto.MatchingGiftEmployer ?? string.Empty,
            Notes = dto.Notes ?? string.Empty,
            ReceiptSent = dto.ReceiptSent,
            ThankYouSent = dto.ThankYouSent,
        };

        /// <summary>
        /// Maps a <see cref="Pledge"/> entity to a <see cref="PledgeDto"/>.
        /// </summary>
        public static PledgeDto ToV2Dto(this Pledge entity) => new()
        {
            Id = entity.Id,
            ContactId = entity.ContactId,
            TotalAmount = entity.TotalAmount,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Frequency = entity.Frequency,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedDate = entity.CreatedDate,
        };

        /// <summary>
        /// Maps a <see cref="PledgeDto"/> to a <see cref="Pledge"/> entity.
        /// </summary>
        public static Pledge ToEntity(this PledgeDto dto) => new()
        {
            Id = dto.Id,
            ContactId = dto.ContactId,
            TotalAmount = dto.TotalAmount,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Frequency = dto.Frequency,
            Status = dto.Status,
            Notes = dto.Notes ?? string.Empty,
        };

        /// <summary>
        /// Maps a <see cref="Grant"/> entity to a <see cref="GrantDto"/>.
        /// </summary>
        public static GrantDto ToV2Dto(this Grant entity) => new()
        {
            Id = entity.Id,
            FunderName = entity.FunderName,
            ProgramName = entity.ProgramName,
            Description = entity.Description,
            AmountMin = entity.AmountMin,
            AmountMax = entity.AmountMax,
            AmountAwarded = entity.AmountAwarded,
            Status = entity.Status,
            SubmissionDeadline = entity.SubmissionDeadline,
            AwardDate = entity.AwardDate,
            ReportingDeadline = entity.ReportingDeadline,
            RenewalDate = entity.RenewalDate,
            FunderContactId = entity.FunderContactId,
            GrantUrl = entity.GrantUrl,
            Notes = entity.Notes,
            CreatedDate = entity.CreatedDate,
        };

        /// <summary>
        /// Maps a <see cref="GrantDto"/> to a <see cref="Grant"/> entity.
        /// </summary>
        public static Grant ToEntity(this GrantDto dto) => new()
        {
            Id = dto.Id,
            FunderName = dto.FunderName ?? string.Empty,
            ProgramName = dto.ProgramName ?? string.Empty,
            Description = dto.Description ?? string.Empty,
            AmountMin = dto.AmountMin,
            AmountMax = dto.AmountMax,
            AmountAwarded = dto.AmountAwarded,
            Status = dto.Status,
            SubmissionDeadline = dto.SubmissionDeadline,
            AwardDate = dto.AwardDate,
            ReportingDeadline = dto.ReportingDeadline,
            RenewalDate = dto.RenewalDate,
            FunderContactId = dto.FunderContactId,
            GrantUrl = dto.GrantUrl ?? string.Empty,
            Notes = dto.Notes ?? string.Empty,
        };

        /// <summary>
        /// Maps a <see cref="GrantTask"/> entity to a <see cref="GrantTaskDto"/>.
        /// </summary>
        public static GrantTaskDto ToV2Dto(this GrantTask entity) => new()
        {
            Id = entity.Id,
            GrantId = entity.GrantId,
            Title = entity.Title,
            DueDate = entity.DueDate,
            IsCompleted = entity.IsCompleted,
            CompletedDate = entity.CompletedDate,
            SortOrder = entity.SortOrder,
            Notes = entity.Notes,
            CreatedDate = entity.CreatedDate,
        };

        /// <summary>
        /// Maps a <see cref="GrantTaskDto"/> to a <see cref="GrantTask"/> entity.
        /// </summary>
        public static GrantTask ToEntity(this GrantTaskDto dto) => new()
        {
            Id = dto.Id,
            GrantId = dto.GrantId,
            Title = dto.Title ?? string.Empty,
            DueDate = dto.DueDate,
            IsCompleted = dto.IsCompleted,
            CompletedDate = dto.CompletedDate,
            SortOrder = dto.SortOrder,
            Notes = dto.Notes ?? string.Empty,
        };
    }
}
