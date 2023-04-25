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
    using System.Formats.Asn1;

    public partial class EditEventPartnerServices
    {
        [Inject]
        public IEventPartnerLocationServiceRestService EventPartnerLocationServiceRestService { get; set; }

        [Inject]
        public IServiceTypeRestService ServiceTypeRestService { get; set; }

        [Inject]
        public IEventPartnerLocationServiceStatusRestService EventPartnerLocationServiceStatusRestService { get; set; }

        [Parameter]
        public DisplayEventPartnerLocation EventPartnerLocation { get; set; }

        [Parameter]
        public Action Refresh { get; set; }

        private List<ServiceType> serviceTypes;
        private List<EventPartnerLocationServiceStatus> eventPartnerLocationServiceStatuses;
        private List<DisplayEventPartnerLocationService> displayEventPartnerLocationServices;

        private const int EventPartnerLocationServiceStatusNone = 0;
        private const int EventPartnerLocationServiceStatusRequested = 1;
        private const int EventPartnerLocationServiceStatusAccepted = 2;
        private const int EventPartnerLocationServiceStatusRemoved = 2;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                displayEventPartnerLocationServices = (await EventPartnerLocationServiceRestService.GetEventPartnerLocationServicesAsync(EventPartnerLocation.EventId, EventPartnerLocation.PartnerLocationId)).ToList();
                serviceTypes = (await ServiceTypeRestService.GetServiceTypesAsync()).ToList();
                eventPartnerLocationServiceStatuses = (await EventPartnerLocationServiceStatusRestService.GetEventPartnerLocationServiceStatusesAsync()).ToList();
            }
            catch (Exception ex)
            {
                if (ex.IsClosedStreamException())
                {
                    return;
                }
            }
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
            displayEventPartnerLocationServices = (await EventPartnerLocationServiceRestService.GetEventPartnerLocationServicesAsync(EventPartnerLocation.EventId, EventPartnerLocation.PartnerLocationId)).ToList();
            Refresh();
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
            displayEventPartnerLocationServices = (await EventPartnerLocationServiceRestService.GetEventPartnerLocationServicesAsync(EventPartnerLocation.EventId, EventPartnerLocation.PartnerLocationId)).ToList();
            Refresh();
            StateHasChanged();
        }
    }
}
