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
    private string partnerLocationName;

    [ObservableProperty]
    private string partnerLocationNotes;

    [ObservableProperty]
    private string serviceName;

    [ObservableProperty]
    private string serviceStatus;

    private int serviceStatusId;

    [ObservableProperty]
    private int serviceTypeId;

    public EventPartnerLocationServiceViewModel(
        IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService)
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
        IsBusy = true;

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

        IsBusy = true;
    }

    [RelayCommand]
    private async Task UnrequestService()
    {
        IsBusy = true;

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

        IsBusy = true;
    }
}