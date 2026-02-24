namespace TrashMobMobile.Services
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface IEventAttendeeMetricsRestService
    {
        Task<EventAttendeeMetrics?> GetMyMetricsAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<EventAttendeeMetrics> SubmitMyMetricsAsync(Guid eventId, EventAttendeeMetrics metrics, CancellationToken cancellationToken = default);

        Task<EventMetricsPublicSummary> GetPublicMetricsAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<int> ApproveAllPendingAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<UserImpactStats> GetUserImpactAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
