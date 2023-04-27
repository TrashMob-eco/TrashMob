namespace TrashMobMobileApp.Features.Events.Components
{
    using Microsoft.AspNetCore.Components;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Features.Pickups;
    
    public partial class PickupLocationList
    {
        private List<PickupLocation> _pickupLocations = new();

        [Parameter]
        public string EventId { get; set; }

        private bool _isLoading;

        [Inject]
        public IMapRestService MapRestService { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        [Inject]
        public IPickupLocationRestService PickupLocationRestService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ReInitializeAsync();
        }

        private async Task ReInitializeAsync()
        {
            _isLoading = true;
            _pickupLocations = (await PickupLocationRestService.GetPickupLocationsAsync(new Guid(EventId))).ToList();
            _isLoading = false;
        }

        private async Task OnAddPickupLocationAsync()
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new AddPickupLocation(MapRestService, PickupLocationRestService, EventId, Refresh));
            await ReInitializeAsync();
        }

        private async void Refresh()
        {
            _pickupLocations = (await PickupLocationRestService.GetPickupLocationsAsync(new Guid(EventId))).ToList();
        }
    }
}
