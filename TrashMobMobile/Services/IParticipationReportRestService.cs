namespace TrashMobMobile.Services
{
    public interface IParticipationReportRestService
    {
        Task RequestReportAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<int> SendAllReportsAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
