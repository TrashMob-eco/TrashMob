using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;

namespace TrashMobMobileApp.Pages.Events.Components
{
    public partial class ActiveEventList
    {
        private List<MobEvent> _mobEventsStatic = new();
        private List<MobEvent> _mobEvents = new();
        private bool _isLoading;
        private MobEvent _selectedEvent;
        private bool _isViewOpen;
        private string _eventSearchText;
        private bool _isButtonLoading;

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            _mobEventsStatic = (await MobEventManager.GetActiveEventsAsync()).ToList();
            _mobEvents = _mobEventsStatic;
            _isLoading = false;
        }

        private void OnViewEventDetails(MobEvent mobEvent)
        {
            _selectedEvent = mobEvent;
            _isViewOpen = !_isViewOpen;
        }

        private async Task OnRegisterAsync(MobEvent mobEvent)
        {
            var currentUser = App.CurrentUser;
            if (currentUser != null)
            {
                var attendee = new EventAttendee
                {
                    UserId = currentUser.Id,
                    EventId = mobEvent.Id
                };

                _isLoading = true;
                await MobEventManager.AddEventAttendeeAsync(attendee);
                _isLoading = false;
                Snackbar.Add($"Registered successfully!", MudBlazor.Severity.Success);
            }
        }

        private void OnSearchTextChanged(string searchText)
        {
            _eventSearchText = searchText;
            if (string.IsNullOrEmpty(_eventSearchText))
            {
                _mobEvents = _mobEventsStatic;
                return;
            }

            _mobEvents = _mobEventsStatic.FindAll(item => item.Name.Contains(_eventSearchText, StringComparison.OrdinalIgnoreCase));
        }
    }
}
