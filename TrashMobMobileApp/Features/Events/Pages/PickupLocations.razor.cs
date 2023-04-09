namespace TrashMobMobileApp.Features.Events.Pages
{
    using Microsoft.AspNetCore.Components;
    using TrashMobMobileApp.StateContainers;

    public partial class PickupLocations
    {
        [Inject]
        public UserStateInformation StateInformation { get; set; }

        protected override void OnInitialized() => TitleContainer.Title = "Pickup Locations";
    }
}
