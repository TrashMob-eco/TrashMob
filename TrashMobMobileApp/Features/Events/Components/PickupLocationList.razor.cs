namespace TrashMobMobileApp.Features.Events.Components
{
    using Microsoft.AspNetCore.Components;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Features.Events.Pages;
    using TrashMobMobileApp.Features.Pickups;
    using TrashMobMobileApp.Shared;
    using TrashMobMobileApp.StateContainers;

    public partial class PickupLocationList
    {
        private List<PickupLocation> _pickupLocations = new();
        private PartnerLocation _haulingPartnerLocation = null;
        
        [Parameter]
        public string EventId { get; set; }

        private bool _isLoading;

        [Inject]
        public UserStateInformation StateInformation { get; set; }

        [Inject]
        public IMapRestService MapRestService { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        [Inject]
        public IPickupLocationRestService PickupLocationRestService { get; set; }

        [Inject]
        public IEventPartnerLocationServiceRestService EventPartnerLocationServiceRestService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ReInitializeAsync();
        }

        private async Task ReInitializeAsync()
        {
            _isLoading = true;
            _haulingPartnerLocation = await EventPartnerLocationServiceRestService.GetHaulingPartnerLocationAsync(new Guid(EventId));
            _pickupLocations = (await PickupLocationRestService.GetPickupLocationsAsync(new Guid(EventId))).ToList();
            _isLoading = false;
        }

        private async Task OnAddPickupLocationAsync()
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new AddPickupLocation(MapRestService, PickupLocationRestService, EventId, Refresh));
            await ReInitializeAsync();
        }

        private async Task OnRemovePickupLocationAsync(PickupLocation pickupLocation)
        {
            await PickupLocationRestService.DeletePickupLocationAsync(pickupLocation);
            await ReInitializeAsync();
        }

        private Task OnEditEventPartnersAsync()
        {
            StateInformation.CurrentlyActiveEditEventTab = EditEvent.EditEventPartnersTab;
            Navigator.NavigateTo(string.Format(Routes.EditEvent, EventId));
            return Task.CompletedTask;
        }

        private async Task OnSubmitPickupLocationsAsync()
        {
            await PickupLocationRestService.SubmitLocationsAsync(new Guid(EventId));
            await ReInitializeAsync();
        }

        private async void Refresh()
        {
            _pickupLocations = (await PickupLocationRestService.GetPickupLocationsAsync(new Guid(EventId))).ToList();
            StateHasChanged();
        }
    }
}
