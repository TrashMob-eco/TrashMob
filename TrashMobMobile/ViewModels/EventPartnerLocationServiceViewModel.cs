namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Data;

public partial class EventPartnerLocationServiceViewModel : BaseViewModel
{
    public EventPartnerLocationServiceViewModel(IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService)
    {
        this.eventPartnerLocationServiceRestService = eventPartnerLocationServiceRestService;
        CanRequestService = true;
        CanUnrequestService = false;
    }

    [ObservableProperty]
    Guid eventId;

    [ObservableProperty]
    Guid partnerLocationId;

    [ObservableProperty]
    int serviceTypeId;

    private int serviceStatusId;

    public int ServiceStatusId
    {
        get
        {
            return serviceStatusId;
        }
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

    [ObservableProperty]
    string partnerLocationName;

    [ObservableProperty]
    string partnerLocationNotes;

    [ObservableProperty]
    string serviceName;

    [ObservableProperty]
    string serviceStatus;

    [ObservableProperty]
    bool canRequestService;

    [ObservableProperty]
    bool canUnrequestService;
    private readonly IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService;

    [RelayCommand]
    private async Task RequestService()
    {
        IsBusy = true;
        
        var eventPartnerLocationService = new EventPartnerLocationService()
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
        
        var eventPartnerLocationService = new EventPartnerLocationService()
        {
            EventId = EventId,
            PartnerLocationId = PartnerLocationId,
            ServiceTypeId = ServiceTypeId,
            EventPartnerLocationServiceStatusId = (int)EventPartnerLocationServiceStatusEnum.Requested,
        };

        await eventPartnerLocationServiceRestService.DeleteEventPartnerLocationServiceAsync(eventPartnerLocationService);

        ServiceStatusId = (int)EventPartnerLocationServiceStatusEnum.None;
        ServiceStatus = EventPartnerLocationServiceStatusEnum.None.ToString();

        IsBusy = true;
    }
}
