using Microsoft.AspNetCore.Components;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;

namespace TrashMobMobileApp.Pages.Events.Components
{
    public partial class AttendingEventList
    {
        private List<MobEvent> _attendingEvents = new();
        private List<MobEvent> _attendingEventsStatic = new();
        private bool _isLoading;
        private string _eventSearchText;
        private bool _isViewOpen;
        private MobEvent _selectedEvent;

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            await GetAttendingEventsAsync();
        }

        private async Task GetAttendingEventsAsync()
        {
            var currentUser = App.CurrentUser;
            if (currentUser != null)
            {
                _isLoading = true;
                _attendingEventsStatic = (await MobEventManager.GetUserEventsAsync(currentUser.Id, true)).ToList();
                _attendingEvents = _attendingEventsStatic;
                _isLoading = false;
            }
        }

        private void OnSearchTextChanged(string searchText)
        {
            _eventSearchText = searchText;
            if (string.IsNullOrEmpty(_eventSearchText))
            {
                _attendingEvents = _attendingEventsStatic;
                return;
            }

            _attendingEvents = _attendingEventsStatic.FindAll(item => item.Name.Contains(_eventSearchText, StringComparison.OrdinalIgnoreCase));
        }

        private void OnViewEventDetails(MobEvent mobEvent)
        {
            _selectedEvent = mobEvent;
            _isViewOpen = !_isViewOpen;
        }

        private async Task OnUnregisterEventAsync(MobEvent mobEvent)
        {
            var currentUser = App.CurrentUser;
            if (currentUser != null)
            {
                var eventAttendee = new EventAttendee
                {
                    EventId = mobEvent.Id,
                    UserId = currentUser.Id
                };

                _isLoading = true;
                await MobEventManager.RemoveEventAttendeeAsync(eventAttendee);
                await GetAttendingEventsAsync();
                _isLoading = false;
            }
        }
    }
}
