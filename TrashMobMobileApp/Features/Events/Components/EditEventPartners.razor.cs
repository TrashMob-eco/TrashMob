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

        private List<DisplayEventPartnerLocation> displayEventPartnerLocations;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _isLoading = true;
                displayEventPartnerLocations = (await EventPartnerLocationServiceRestService.GetEventPartnerLocationsAsync(Event.Id)).ToList();
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
    }
}
