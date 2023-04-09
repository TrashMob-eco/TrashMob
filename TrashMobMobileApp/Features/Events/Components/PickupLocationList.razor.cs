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
        public string EventId
        {
            get
            {
                return eventId.ToString();
            }
            set
            {
                eventId = new Guid(value);
            }
        }

        private Guid eventId;

        private bool _isLoading;

        [Inject]
        public IMapRestService MapRestService { get; set; }

        [Inject]
        public IPickupLocationRestService PickupLocationRestService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ReInitializeAsync();
        }

        private async Task ReInitializeAsync()
        {
            _isLoading = true;
            _pickupLocations = (await PickupLocationRestService.GetPickupLocationsAsync(eventId)).ToList();
            _isLoading = false;
        }

        private async Task OnAddPickupLocationAsync()
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new AddPickupLocation(MapRestService, PickupLocationRestService, eventId));
            await ReInitializeAsync();
        }
    }
}
