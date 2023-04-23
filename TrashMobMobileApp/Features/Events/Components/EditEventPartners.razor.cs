namespace TrashMobMobileApp.Features.Events.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using TrashMobMobileApp.Data;
    using TrashMob.Models;
    using TrashMobMobileApp.Extensions;
    using TrashMob.Models.Poco;

    public partial class EditEventPartners
    {
        private bool _isLoading;

        [Inject]
        public IEventPartnerLocationServiceRestService EventPartnerLocationServiceRestService { get; set; }

        [Inject]
        public IServiceTypeRestService ServiceTypeRestService { get; set; }

        [Inject]
        public IEventPartnerLocationServiceStatusRestService EventPartnerLocationServiceStatusRestService { get; set; }

        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public EventCallback<Event> EventChanged { get; set; }

        [Parameter]
        public EventCallback OnStepFinished { get; set; }

        private List<DisplayEventPartnerLocation> displayEventPartnerLocations;
        private List<ServiceType> serviceTypes;
        private List<EventPartnerLocationServiceStatus> eventPartnerLocationServiceStatuses;
        private List<DisplayEventPartnerLocationService> displayEventPartnerLocationServices;

        private const int EventPartnerLocationServiceStatusNone = 0;
        private const int EventPartnerLocationServiceStatusRequested = 1;
        private const int EventPartnerLocationServiceStatusAccepted = 2;
        private const int EventPartnerLocationServiceStatusDeclined = 3;
        private const int EventPartnerLocationServiceStatusRemoved = 2;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _isLoading = true;
                displayEventPartnerLocations = (await EventPartnerLocationServiceRestService.GetEventPartnerLocationsAsync(Event.Id)).ToList();
                serviceTypes = (await ServiceTypeRestService.GetServiceTypesAsync()).ToList();
                eventPartnerLocationServiceStatuses = (await EventPartnerLocationServiceStatusRestService.GetEventPartnerLocationServiceStatusesAsync()).ToList();
                _isLoading = false;
            }
            catch (Exception ex)
            {
                if (ex.IsClosedStreamException())
                {
                    return;
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async void OnViewLocationServices(DisplayEventPartnerLocation displayEventPartnerLocation)
        {
            displayEventPartnerLocationServices = (await EventPartnerLocationServiceRestService.GetEventPartnerLocationServicesAsync(displayEventPartnerLocation.EventId, displayEventPartnerLocation.PartnerLocationId)).ToList();
            StateHasChanged();
        }

        private async void OnRequestService(DisplayEventPartnerLocationService displayEventPartnerLocationService)
        {
            var eventPartnerLocationService = new EventPartnerLocationService
            {
                EventId = displayEventPartnerLocationService.EventId,
                PartnerLocationId = displayEventPartnerLocationService.PartnerLocationId,
                ServiceTypeId = displayEventPartnerLocationService.ServiceTypeId,
                EventPartnerLocationServiceStatusId = EventPartnerLocationServiceStatusRequested
            };

            await EventPartnerLocationServiceRestService.AddEventPartnerLocationService(eventPartnerLocationService);
            StateHasChanged();
        }

        private async void OnRemoveService(DisplayEventPartnerLocationService displayEventPartnerLocationService)
        {
            var eventPartnerLocationService = new EventPartnerLocationService
            {
                EventId = displayEventPartnerLocationService.EventId,
                PartnerLocationId = displayEventPartnerLocationService.PartnerLocationId,
                ServiceTypeId = displayEventPartnerLocationService.ServiceTypeId,
                EventPartnerLocationServiceStatusId = EventPartnerLocationServiceStatusRemoved
            };

            await EventPartnerLocationServiceRestService.DeleteEventPartnerLocationServiceAsync(eventPartnerLocationService);
            StateHasChanged();
        }
    }
}
