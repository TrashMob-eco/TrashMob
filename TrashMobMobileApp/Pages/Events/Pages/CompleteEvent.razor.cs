using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMob.Models;
using TrashMobMobileApp.Data;

namespace TrashMobMobileApp.Pages.Events.Pages
{
    public partial class CompleteEvent
    {
        MudForm _completeEventForm;
        private bool _success;
        private string[] _errors;
        private Event _event;
        private EventSummary _eventSummary = new();
        private bool _isLoading;

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Parameter]
        public string EventId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TitleContainer.Title = "Complete Event";
            _isLoading = true;
            _event = await MobEventManager.GetEventAsync(Guid.Parse(EventId));
            _isLoading = false;
        }

        private async Task OnSubmitAsync()
        {
            await _completeEventForm?.Validate();
            if (_success)
            {
                _eventSummary.EventId = _event.Id;
                _eventSummary.CreatedByUserId = App.CurrentUser.Id;
                _eventSummary.CreatedDate = DateTimeOffset.UtcNow;
                _eventSummary.LastUpdatedByUserId = App.CurrentUser.Id;
                _isLoading = true;
                await MobEventManager.AddEventSummaryAsync(_eventSummary);
                _event.EventStatusId = 4;
                await MobEventManager.UpdateEventAsync(_event);
                _isLoading = false;
                Snackbar.Add("Event completed!", Severity.Success);
            }
        }
    }
}
