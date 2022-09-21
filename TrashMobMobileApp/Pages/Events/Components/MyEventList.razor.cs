using Microsoft.AspNetCore.Components;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;
using TrashMobMobileApp.Shared;

namespace TrashMobMobileApp.Pages.Events.Components
{
    public partial class MyEventList
    {
        private List<MobEvent> _myEvents = new();
        private List<MobEvent> _myEventsStatic = new();
        private bool _isLoading;
        private string _eventSearchText;
        private bool _isViewOpen;
        private MobEvent _selectedEvent;

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            await GetMyEventsAsync();
        }

        private async Task GetMyEventsAsync()
        {
            var currentUser = App.CurrentUser;
            if (currentUser != null)
            {
                _isLoading = true;
                _myEventsStatic = (await MobEventManager.GetUserEventsAsync(currentUser.Id, true))
                    .Where(item => item.CreatedByUserId == currentUser.Id).ToList();
                _myEvents = _myEventsStatic;
                _isLoading = false;
            }
        }

        private void OnSearchTextChanged(string searchText)
        {
            _eventSearchText = searchText;
            if (string.IsNullOrEmpty(_eventSearchText))
            {
                _myEvents = _myEventsStatic;
                return;
            }

            _myEvents = _myEventsStatic.FindAll(item => item.Name.Contains(_eventSearchText, StringComparison.OrdinalIgnoreCase));
        }

        private void OnCreateEvent() => Navigator.NavigateTo(Routes.CreateEvent);

        private void OnViewEventDetails(MobEvent mobEvent)
        {
            _selectedEvent = mobEvent;
            _isViewOpen = !_isViewOpen;
        }

        private void OnCancelEvent(MobEvent mobEvent) 
            => Navigator.NavigateTo(string.Format(Routes.CancelEvent, mobEvent.Id.ToString()));

        private void OnEdit(MobEvent mobEvent)
            => Navigator.NavigateTo(string.Format(Routes.EditEvent, mobEvent.Id));
    }
}
