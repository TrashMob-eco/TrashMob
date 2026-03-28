namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for dependents.
    /// </summary>
    public static class DependentMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="Dependent"/> to a <see cref="DependentDto"/>.
        /// </summary>
        public static DependentDto ToV2Dto(this Dependent entity)
        {
            return new DependentDto
            {
                Id = entity.Id,
                ParentUserId = entity.ParentUserId,
                FirstName = entity.FirstName ?? string.Empty,
                LastName = entity.LastName ?? string.Empty,
                DateOfBirth = entity.DateOfBirth,
                Relationship = entity.Relationship ?? string.Empty,
                MedicalNotes = entity.MedicalNotes ?? string.Empty,
                EmergencyContactPhone = entity.EmergencyContactPhone ?? string.Empty,
                IsActive = entity.IsActive,
                PrivoConsentStatus = entity.ParentalConsent != null ? (int)entity.ParentalConsent.Status : null,
            };
        }

        /// <summary>
        /// Maps a <see cref="DependentDto"/> to a <see cref="Dependent"/> entity.
        /// </summary>
        public static Dependent ToEntity(this DependentDto dto)
        {
            return new Dependent
            {
                Id = dto.Id,
                ParentUserId = dto.ParentUserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                Relationship = dto.Relationship,
                MedicalNotes = dto.MedicalNotes,
                EmergencyContactPhone = dto.EmergencyContactPhone,
                IsActive = dto.IsActive,
            };
        }
    }
}
