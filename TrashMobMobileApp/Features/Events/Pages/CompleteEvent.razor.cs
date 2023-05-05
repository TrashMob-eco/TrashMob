namespace TrashMobMobileApp.Features.Events.Pages
{
    using Microsoft.AspNetCore.Components;
    using TrashMobMobileApp.StateContainers;

    public partial class CompleteEvent
    {
        [Inject]
        public UserStateInformation StateInformation { get; set; }

        [Parameter]
        public string EventId { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        protected override void OnInitialized() => TitleContainer.Title = "Event Summary";
    }
}
