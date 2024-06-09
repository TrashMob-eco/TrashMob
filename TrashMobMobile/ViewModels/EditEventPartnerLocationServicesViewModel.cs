namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TrashMobMobile.Services;

public partial class EditEventPartnerLocationServicesViewModel : BaseViewModel
{
    private readonly IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService;
    private readonly IEventPartnerLocationServiceStatusRestService eventPartnerLocationServiceStatusRestService;
    private readonly IServiceTypeRestService serviceTypeRestService;

    [ObservableProperty]
    private EventPartnerLocationServiceViewModel selectedEventPartnerLocationServiceViewModel;

    public EditEventPartnerLocationServicesViewModel(
        IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService,
        IServiceTypeRestService serviceTypeRestService,
        IEventPartnerLocationServiceStatusRestService eventPartnerLocationServiceStatusRestService)
    {
        this.eventPartnerLocationServiceRestService = eventPartnerLocationServiceRestService;
        this.serviceTypeRestService = serviceTypeRestService;
        this.eventPartnerLocationServiceStatusRestService = eventPartnerLocationServiceStatusRestService;
    }

    public ObservableCollection<EventPartnerLocationServiceViewModel> EventPartnerLocationServices { get; set; } =
        new();

    public async Task Init(Guid eventId, Guid partnerLocationId)
    {
        IsBusy = true;

        var serviceTypes = await serviceTypeRestService.GetServiceTypesAsync();
        var serviceStatuses =
            await eventPartnerLocationServiceStatusRestService.GetEventPartnerLocationServiceStatusesAsync();

        EventPartnerLocationServices.Clear();

        var eventPartnerLocationServices =
            await eventPartnerLocationServiceRestService.GetEventPartnerLocationServicesAsync(eventId,
                partnerLocationId);

        foreach (var eventPartnerLocationService in eventPartnerLocationServices)
        {
            var eventPartnerLocationServiceViewModel =
                new EventPartnerLocationServiceViewModel(eventPartnerLocationServiceRestService)
                {
                    EventId = eventId,
                    PartnerLocationId = partnerLocationId,
                    ServiceTypeId = eventPartnerLocationService.ServiceTypeId,
                    ServiceStatusId = eventPartnerLocationService.EventPartnerLocationServiceStatusId,
                    PartnerLocationName = eventPartnerLocationService.PartnerLocationName,
                    PartnerLocationNotes = eventPartnerLocationService.PartnerLocationServicePublicNotes,
                    ServiceName = serviceTypes.First(st => st.Id == eventPartnerLocationService.ServiceTypeId).Name,
                    ServiceStatus = serviceStatuses.First(ss =>
                        ss.Id == eventPartnerLocationService.EventPartnerLocationServiceStatusId).Name,
                };

            EventPartnerLocationServices.Add(eventPartnerLocationServiceViewModel);
        }

        IsBusy = false;
    }
}