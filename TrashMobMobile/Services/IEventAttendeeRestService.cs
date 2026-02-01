namespace TrashMobMobile.Services
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface IEventAttendeeRestService
    {
        Task<IEnumerable<DisplayUser>> GetEventAttendeesAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        Task AddAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default);

        Task RemoveAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default);

        Task<IEnumerable<DisplayUser>> GetEventLeadsAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<EventAttendee> PromoteToLeadAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);

        Task<EventAttendee> DemoteFromLeadAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);
    }
}