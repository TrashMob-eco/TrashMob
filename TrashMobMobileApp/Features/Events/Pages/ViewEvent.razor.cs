using Microsoft.AspNetCore.Components;
using TrashMob.Models;
using TrashMobMobileApp.Data;

namespace TrashMobMobileApp.Features.Events.Pages
{
    public partial class ViewEvent
    {
        private bool _isLoading;
        private Event _event = new();
        private EventType _eventType = new();

        [Parameter]
        public string EventId { get; set; }

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Inject]
        public IEventTypeRestService EventTypeManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _isLoading = true;
            await GetEventDetails();
            _isLoading = false;
        }

        //String.Format("{0:dddd, MMMM d, yyyy}", dt);
        private async Task GetEventDetails()
        {
            _event = await MobEventManager.GetEventAsync(Guid.Parse(EventId));
            var eventTypes = await EventTypeManager.GetEventTypesAsync();
            if (eventTypes != null && eventTypes.Any())
            {
                _eventType = eventTypes.First(item => item.Id == _event.EventTypeId);
            }
        }
    }
}
