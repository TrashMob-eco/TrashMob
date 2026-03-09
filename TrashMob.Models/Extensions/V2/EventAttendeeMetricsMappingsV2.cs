namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for event attendee metrics.
    /// </summary>
    public static class EventAttendeeMetricsMappingsV2
    {
        /// <summary>
        /// Maps an <see cref="EventAttendeeMetrics"/> to an <see cref="EventAttendeeMetricsDto"/>.
        /// </summary>
        public static EventAttendeeMetricsDto ToV2Dto(this EventAttendeeMetrics entity)
        {
            return new EventAttendeeMetricsDto
            {
                Id = entity.Id,
                EventId = entity.EventId,
                UserId = entity.UserId,
                BagsCollected = entity.BagsCollected,
                PickedWeight = entity.PickedWeight,
                PickedWeightUnitId = entity.PickedWeightUnitId,
                DurationMinutes = entity.DurationMinutes,
                Notes = entity.Notes ?? string.Empty,
                Status = entity.Status ?? string.Empty,
                ReviewedDate = entity.ReviewedDate,
                RejectionReason = entity.RejectionReason ?? string.Empty,
                AdjustedBagsCollected = entity.AdjustedBagsCollected,
                AdjustedPickedWeight = entity.AdjustedPickedWeight,
                AdjustedPickedWeightUnitId = entity.AdjustedPickedWeightUnitId,
                AdjustedDurationMinutes = entity.AdjustedDurationMinutes,
                AdjustmentReason = entity.AdjustmentReason ?? string.Empty,
            };
        }
    }
}
