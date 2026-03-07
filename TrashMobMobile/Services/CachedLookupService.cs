namespace TrashMobMobile.Services;

using TrashMob.Models;

/// <summary>
/// In-memory cache wrapper for static lookup data (EventTypes, ServiceTypes, etc.).
/// Fetches once per app session and reuses for the lifetime of the singleton.
/// </summary>
public class CachedLookupService(IHttpClientFactory httpClientFactory)
    : IEventTypeRestService, IServiceTypeRestService, IEventPartnerLocationServiceStatusRestService
{
    private readonly EventTypeRestService eventTypeService = new(httpClientFactory);
    private readonly ServiceTypeRestService serviceTypeService = new(httpClientFactory);
    private readonly EventPartnerLocationServiceStatusRestService partnerLocationStatusService = new(httpClientFactory);

    private List<EventType>? cachedEventTypes;
    private List<ServiceType>? cachedServiceTypes;
    private List<EventPartnerLocationServiceStatus>? cachedPartnerLocationStatuses;

    private readonly SemaphoreSlim eventTypeLock = new(1, 1);
    private readonly SemaphoreSlim serviceTypeLock = new(1, 1);
    private readonly SemaphoreSlim partnerLocationStatusLock = new(1, 1);

    public async Task<IEnumerable<EventType>> GetEventTypesAsync(CancellationToken cancellationToken = default)
    {
        if (cachedEventTypes != null)
            return cachedEventTypes;

        await eventTypeLock.WaitAsync(cancellationToken);
        try
        {
            if (cachedEventTypes != null)
                return cachedEventTypes;

            cachedEventTypes = (await eventTypeService.GetEventTypesAsync(cancellationToken)).ToList();
            return cachedEventTypes;
        }
        finally
        {
            eventTypeLock.Release();
        }
    }

    public async Task<IEnumerable<ServiceType>> GetServiceTypesAsync(CancellationToken cancellationToken = default)
    {
        if (cachedServiceTypes != null)
            return cachedServiceTypes;

        await serviceTypeLock.WaitAsync(cancellationToken);
        try
        {
            if (cachedServiceTypes != null)
                return cachedServiceTypes;

            cachedServiceTypes = (await serviceTypeService.GetServiceTypesAsync(cancellationToken)).ToList();
            return cachedServiceTypes;
        }
        finally
        {
            serviceTypeLock.Release();
        }
    }

    public async Task<IEnumerable<EventPartnerLocationServiceStatus>> GetEventPartnerLocationServiceStatusesAsync(
        CancellationToken cancellationToken = default)
    {
        if (cachedPartnerLocationStatuses != null)
            return cachedPartnerLocationStatuses;

        await partnerLocationStatusLock.WaitAsync(cancellationToken);
        try
        {
            if (cachedPartnerLocationStatuses != null)
                return cachedPartnerLocationStatuses;

            cachedPartnerLocationStatuses = (await partnerLocationStatusService
                .GetEventPartnerLocationServiceStatusesAsync(cancellationToken)).ToList();
            return cachedPartnerLocationStatuses;
        }
        finally
        {
            partnerLocationStatusLock.Release();
        }
    }
}
