using Microsoft.AspNetCore.Components;
using TrashMobMobileApp.Data;
using TrashMob.Models;

namespace TrashMobMobileApp.Features.Events.Components
{
    public partial class CreateEventStep6
    {
        private bool _isLoading;
        private bool _isCreated;

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public EventCallback<Event> EventChanged { get; set; }

        [Parameter]
        public EventCallback OnStepFinished { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TitleContainer.Title = "Create Event (5/5)";
            Event.CreatedByUserId = App.CurrentUser.Id;
            Event.LastUpdatedByUserId = App.CurrentUser.Id;
            Event.LastUpdatedDate = DateTime.Now;
            Event.EventStatusId = 1;
            _isLoading = true;
            var result = await MobEventManager.AddEventAsync(Event);
            _isLoading = false;
            if (result != null)
            {
                _isCreated = true;
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
