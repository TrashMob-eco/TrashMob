namespace TrashMobMobileApp.Features.Events.Components
{
    using Microsoft.AspNetCore.Components;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Features.Pickups;
    
    public partial class PickupLocationList
    {
        private List<PickupLocation> _pickupLocations = new();

        private string eventId = string.Empty;

        private bool _isLoading;

        [Inject]
        public IMapRestService MapRestService { get; set; }

        [Inject]
        public IPickupLocationRestService PickupLocationRestService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            eventId = EventContainer.EventId;
            await ReInitializeAsync();
        }

        private async Task ReInitializeAsync()
        {
            _isLoading = true;
            _pickupLocations = (await PickupLocationRestService.GetPickupLocationsAsync(new Guid(eventId))).ToList();
            _isLoading = false;
        }

        private async Task OnAddPickupLocationAsync()
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new AddPickupLocation(MapRestService, PickupLocationRestService, eventId));
            await ReInitializeAsync();
        }
    }
}
