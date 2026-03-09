namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for <see cref="EventSummary"/>.
    /// </summary>
    public static class EventSummaryMappingsV2
    {
        /// <summary>
        /// Maps an <see cref="EventSummary"/> entity to a <see cref="EventSummaryDto"/>.
        /// </summary>
        /// <param name="entity">The event summary entity.</param>
        /// <returns>A V2 event summary DTO.</returns>
        public static EventSummaryDto ToV2Dto(this EventSummary entity)
        {
            return new EventSummaryDto
            {
                EventId = entity.EventId,
                NumberOfBuckets = entity.NumberOfBuckets,
                NumberOfBags = entity.NumberOfBags,
                DurationInMinutes = entity.DurationInMinutes,
                ActualNumberOfAttendees = entity.ActualNumberOfAttendees,
                PickedWeight = entity.PickedWeight,
                PickedWeightUnitId = entity.PickedWeightUnitId,
                IsFromRouteData = entity.IsFromRouteData,
                Notes = entity.Notes ?? string.Empty,
                CreatedDate = entity.CreatedDate.GetValueOrDefault(),
                LastUpdatedDate = entity.LastUpdatedDate.GetValueOrDefault(),
            };
        }
    }
}
