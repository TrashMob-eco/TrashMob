#nullable enable

namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping Stats to V2 DTOs.
    /// </summary>
    public static class StatsMappingsV2
    {
        /// <summary>
        /// Maps a Stats object to a V2 StatsDto.
        /// </summary>
        public static StatsDto ToV2Dto(this Stats stats)
        {
            return new StatsDto
            {
                TotalBags = stats.TotalBags,
                TotalHours = stats.TotalHours,
                TotalEvents = stats.TotalEvents,
                TotalWeightInPounds = stats.TotalWeightInPounds,
                TotalWeightInKilograms = stats.TotalWeightInKilograms,
                TotalParticipants = stats.TotalParticipants,
                TotalLitterReportsSubmitted = stats.TotalLitterReportsSubmitted,
                TotalLitterReportsClosed = stats.TotalLitterReportsClosed,
            };
        }

        /// <summary>
        /// Maps a V2 StatsDto back to a Stats object.
        /// </summary>
        public static Stats ToEntity(this StatsDto dto)
        {
            return new Stats
            {
                TotalBags = dto.TotalBags,
                TotalHours = dto.TotalHours,
                TotalEvents = dto.TotalEvents,
                TotalWeightInPounds = dto.TotalWeightInPounds,
                TotalWeightInKilograms = dto.TotalWeightInKilograms,
                TotalParticipants = dto.TotalParticipants,
                TotalLitterReportsSubmitted = dto.TotalLitterReportsSubmitted,
                TotalLitterReportsClosed = dto.TotalLitterReportsClosed,
            };
        }
    }
}
