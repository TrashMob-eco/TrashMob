#nullable enable

namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping DependentWaiver entities to V2 DTOs.
    /// </summary>
    public static class DependentWaiverMappingsV2
    {
        /// <summary>
        /// Maps a DependentWaiver entity to a V2 DependentWaiverDto.
        /// Excludes internal tracking fields (IP address, user agent).
        /// </summary>
        public static DependentWaiverDto ToV2Dto(this DependentWaiver entity)
        {
            return new DependentWaiverDto
            {
                Id = entity.Id,
                DependentId = entity.DependentId,
                WaiverVersionId = entity.WaiverVersionId,
                SignedByUserId = entity.SignedByUserId,
                TypedLegalName = entity.TypedLegalName ?? string.Empty,
                AcceptedDate = entity.AcceptedDate,
                ExpiryDate = entity.ExpiryDate,
                DocumentUrl = entity.DocumentUrl ?? string.Empty,
            };
        }
    }
}
