namespace TrashMobMobileApp.Features.Events.Components
{
    using Microsoft.AspNetCore.Components;
    using TrashMobMobileApp.Data;
    using TrashMob.Models;
    using TrashMobMobileApp.Extensions;
    using TrashMob.Models.Poco;

    public partial class ManageEventPartners
    {
        private bool _isLoading;
        private bool _isCreated;

        [Inject]
        public IEventPartnerLocationServiceRestService EventPartnerLocationServiceRestService { get; set; }

        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public EventCallback OnStepFinished { get; set; }

        private IEnumerable<DisplayEventPartnerLocation> displayEventPartnerLocations;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                TitleContainer.Title = "Manage Partners for Event";
                _isLoading = true;
                displayEventPartnerLocations = await EventPartnerLocationServiceRestService.GetEventPartnerLocationsAsync(Event.Id);
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

        private async Task OnStepFinishedAsync()
        {
            if (OnStepFinished.HasDelegate)
            {
                await OnStepFinished.InvokeAsync();
            }
        }
    }
}
