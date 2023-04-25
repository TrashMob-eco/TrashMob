namespace TrashMobMobileApp.Features.Events.Pages
{
    using Microsoft.AspNetCore.Components;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.StateContainers;

    public partial class EditEvent
    {
        [Inject]
        public UserStateInformation StateInformation { get; set; }

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Parameter]
        public string EventId { get; set; }

        public Event Event { get; set; }

        protected override void OnInitialized() => TitleContainer.Title = "Edit Event";

        protected override async Task OnInitializedAsync()
        {
            StateInformation.CurrentlyActiveEditEventTab = 0;
            Event = await MobEventManager.GetEventAsync(Guid.Parse(EventId));
            await base.OnInitializedAsync();
        }
    }
}