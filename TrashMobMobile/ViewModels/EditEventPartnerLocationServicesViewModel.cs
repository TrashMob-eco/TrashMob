namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TrashMobMobile.Services;

public partial class EditEventPartnerLocationServicesViewModel(
    IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService,
    IServiceTypeRestService serviceTypeRestService,
    IEventPartnerLocationServiceStatusRestService eventPartnerLocationServiceStatusRestService,
    INotificationService notificationService)
        : BaseViewModel(notificationService)
{
    private readonly IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService = eventPartnerLocationServiceRestService;
    private readonly IEventPartnerLocationServiceStatusRestService eventPartnerLocationServiceStatusRestService = eventPartnerLocationServiceStatusRestService;
    private readonly IServiceTypeRestService serviceTypeRestService = serviceTypeRestService;

    [ObservableProperty]
    private EventPartnerLocationServiceViewModel selectedEventPartnerLocationServiceViewModel = null!;

    public ObservableCollection<EventPartnerLocationServiceViewModel> EventPartnerLocationServices { get; set; } =
        new();

    public async Task Init(Guid eventId, Guid partnerLocationId)
    {
        await ExecuteAsync(async () =>
        {
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
                    new EventPartnerLocationServiceViewModel(eventPartnerLocationServiceRestService, NotificationService)
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
        }, "An error has occurred while loading the event partner location services. Please wait and try again in a moment.");
    }
}