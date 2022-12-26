using Microsoft.AspNetCore.Components;
using TrashMobMobileApp.StateContainers;

namespace TrashMobMobileApp.Features.Events.Pages
{
    public partial class Events
    {
        [Inject]
        public UserStateInformation StateInformation { get; set; }

        protected override void OnInitialized() => TitleContainer.Title = "Events";
    }
}
