namespace TrashMobMobileApp.Features.Events.Pages
{
    using Microsoft.AspNetCore.Components;
    using TrashMobMobileApp.StateContainers;

    public partial class Events
    {
        [Inject]
        public UserStateInformation StateInformation { get; set; }

        protected override void OnInitialized() => TitleContainer.Title = "Events";
    }
}
