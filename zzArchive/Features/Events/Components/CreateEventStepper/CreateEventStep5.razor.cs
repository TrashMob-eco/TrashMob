namespace TrashMobMobileApp.Features.Events.Components
{
    using Microsoft.AspNetCore.Components;
    using TrashMobMobileApp.Data;
    using TrashMob.Models;
    using TrashMobMobileApp.Extensions;

    public partial class CreateEventStep5
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
            try
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
                    Event.Id = result.Id;
                    _isCreated = true;
                }
            }
            catch (Exception ex)
            {
                if (ex.IsClosedStreamException())
                {
                    return;
                }
            }
            finally
            {
                _isLoading = false;
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
