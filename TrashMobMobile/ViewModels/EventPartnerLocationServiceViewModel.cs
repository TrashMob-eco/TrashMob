namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Services;

public partial class EventPartnerLocationServiceViewModel : BaseViewModel
{
    private readonly IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService;

    [ObservableProperty]
    private bool canRequestService;

    [ObservableProperty]
    private bool canUnrequestService;

    [ObservableProperty]
    private Guid eventId;

    [ObservableProperty]
    private Guid partnerLocationId;

    [ObservableProperty]
    private string partnerLocationName = string.Empty;

    [ObservableProperty]
    private string partnerLocationNotes = string.Empty;

    [ObservableProperty]
    private string serviceName = string.Empty;

    [ObservableProperty]
    private string serviceStatus = string.Empty;

    private int serviceStatusId;

    [ObservableProperty]
    private int serviceTypeId;

    public EventPartnerLocationServiceViewModel(
        IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService, INotificationService notificationService) : base(notificationService)
    {
        this.eventPartnerLocationServiceRestService = eventPartnerLocationServiceRestService;
        CanRequestService = true;
        CanUnrequestService = false;
    }

    public int ServiceStatusId
    {
        get => serviceStatusId;
        set
        {
            if (serviceStatusId != value)
            {
                serviceStatusId = value;

                CanRequestService = value == (int)EventPartnerLocationServiceStatusEnum.None;
                CanUnrequestService = value == (int)EventPartnerLocationServiceStatusEnum.Requested;
            }
        }
    }

    [RelayCommand]
    private async Task RequestService()
    {
        await ExecuteAsync(async () =>
        {
            var eventPartnerLocationService = new EventPartnerLocationService
            {
                EventId = EventId,
                PartnerLocationId = PartnerLocationId,
                ServiceTypeId = ServiceTypeId,
                EventPartnerLocationServiceStatusId = (int)EventPartnerLocationServiceStatusEnum.Requested,
            };

            await eventPartnerLocationServiceRestService.AddEventPartnerLocationService(eventPartnerLocationService);

            ServiceStatusId = (int)EventPartnerLocationServiceStatusEnum.Requested;
            ServiceStatus = EventPartnerLocationServiceStatusEnum.Requested.ToString();
        }, "An error has occurred while requesting the service. Please wait and try again in a moment.");
    }

    [RelayCommand]
    private async Task UnrequestService()
    {
        await ExecuteAsync(async () =>
        {
            var eventPartnerLocationService = new EventPartnerLocationService
            {
                EventId = EventId,
                PartnerLocationId = PartnerLocationId,
                ServiceTypeId = ServiceTypeId,
                EventPartnerLocationServiceStatusId = (int)EventPartnerLocationServiceStatusEnum.Requested,
            };

            await eventPartnerLocationServiceRestService
                .DeleteEventPartnerLocationServiceAsync(eventPartnerLocationService);

            ServiceStatusId = (int)EventPartnerLocationServiceStatusEnum.None;
            ServiceStatus = EventPartnerLocationServiceStatusEnum.None.ToString();
        }, "An error has occurred while removing the service. Please wait and try again in a moment.");
    }
}