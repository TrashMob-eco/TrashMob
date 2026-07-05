namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 DTO mappings for the sales report subscribers list (Project 63 Phase 4b).
    /// </summary>
    public static class SalesReportSubscriberMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="SalesReportSubscriber"/> to a <see cref="SalesReportSubscriberDto"/>.
        /// Assumes the caller has eager-loaded the <see cref="SalesReportSubscriber.User"/>
        /// navigation.
        /// </summary>
        public static SalesReportSubscriberDto ToV2Dto(this SalesReportSubscriber entity) => new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            UserName = entity.User?.UserName,
            Email = entity.User?.Email,
            IncludeWeekly = entity.IncludeWeekly,
            IncludeMonthly = entity.IncludeMonthly,
        };
    }
}
