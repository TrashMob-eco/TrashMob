namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMobMobile.Data;

public partial class EditEventPartnerLocationServicesViewModel :  BaseViewModel
{
    private readonly IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService;
    private readonly IServiceTypeRestService serviceTypeRestService;
    private readonly IEventPartnerLocationServiceStatusRestService eventPartnerLocationServiceStatusRestService;

    public EditEventPartnerLocationServicesViewModel(IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService,
                                                     IServiceTypeRestService serviceTypeRestService,
                                                     IEventPartnerLocationServiceStatusRestService eventPartnerLocationServiceStatusRestService)
    {
        SaveEventPartnerLocationServicesCommand = new Command(async () => await SaveEventPartnerLocationServices());
        this.eventPartnerLocationServiceRestService = eventPartnerLocationServiceRestService;
        this.serviceTypeRestService = serviceTypeRestService;
        this.eventPartnerLocationServiceStatusRestService = eventPartnerLocationServiceStatusRestService;
    }

    [ObservableProperty]
    EventPartnerLocationServiceViewModel selectedEventPartnerLocationServiceViewModel;

    public ObservableCollection<EventPartnerLocationServiceViewModel> EventPartnerLocationServices { get; set; } = new ObservableCollection<EventPartnerLocationServiceViewModel>();

    public async Task Init(Guid eventId, Guid partnerLocationId)
    {
        IsBusy = true;

        var serviceTypes = await serviceTypeRestService.GetServiceTypesAsync();
        var serviceStatuses = await eventPartnerLocationServiceStatusRestService.GetEventPartnerLocationServiceStatusesAsync();

        EventPartnerLocationServices.Clear();

        var eventPartnerLocationServices = await eventPartnerLocationServiceRestService.GetEventPartnerLocationServicesAsync(eventId, partnerLocationId);

        foreach (var eventPartnerLocationService in eventPartnerLocationServices)
        {
            var eventPartnerLocationServiceViewModel = new EventPartnerLocationServiceViewModel
            {
                PartnerLocationName = eventPartnerLocationService.PartnerLocationName,
                PartnerLocationNotes = eventPartnerLocationService.PartnerLocationServicePublicNotes,
                ServiceName = serviceTypes.First(st => st.Id == eventPartnerLocationService.ServiceTypeId).Name,
                ServiceStatus = serviceStatuses.First(ss => ss.Id == eventPartnerLocationService.EventPartnerLocationServiceStatusId).Name
            };

            EventPartnerLocationServices.Add(eventPartnerLocationServiceViewModel);
        }

        IsBusy = false;
    }

    public ICommand SaveEventPartnerLocationServicesCommand { get; set; }

    private async Task SaveEventPartnerLocationServices()
    {
        IsBusy = true;

        IsBusy = false;

        await Notify("Event has been saved.");
    }
}
