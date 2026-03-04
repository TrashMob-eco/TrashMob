namespace TrashMobMobile.Services
{
    using TrashMob.Models;

    public interface IDependentRestService
    {
        // Dependent CRUD (api/users/{userId}/dependents)
        Task<List<Dependent>> GetDependentsAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<Dependent> AddDependentAsync(Guid userId, Dependent dependent, CancellationToken cancellationToken = default);

        Task<Dependent> UpdateDependentAsync(Guid userId, Dependent dependent, CancellationToken cancellationToken = default);

        Task DeleteDependentAsync(Guid userId, Guid dependentId, CancellationToken cancellationToken = default);

        // Dependent Waivers (api/dependents/{dependentId}/waiver)
        Task<DependentWaiver> SignWaiverAsync(Guid dependentId, Guid waiverVersionId, string typedLegalName, CancellationToken cancellationToken = default);

        Task<DependentWaiver> GetCurrentWaiverAsync(Guid dependentId, CancellationToken cancellationToken = default);

        // Event Dependents (api/events/{eventId}/dependents)
        Task<List<EventDependent>> RegisterDependentsForEventAsync(Guid eventId, List<Guid> dependentIds, CancellationToken cancellationToken = default);

        Task<List<EventDependent>> GetEventDependentsAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<int> GetEventDependentCountAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task UnregisterDependentFromEventAsync(Guid eventId, Guid dependentId, CancellationToken cancellationToken = default);
    }
}
