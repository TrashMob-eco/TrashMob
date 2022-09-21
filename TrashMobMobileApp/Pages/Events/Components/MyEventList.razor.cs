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
                //var randomEvents = new List<MobEvent>
                //{
                //    new MobEvent
                //    {
                //        Name = "Cannon Beach",
                //        Description = "Garbage collect at cannon beach"
                //    },
                //    new MobEvent
                //    {
                //        Name = "Lincoln Beach",
                //        Description = "Meet and help clean lincoln beach"
                //    }
                //};
                //_myEvents.AddRange(randomEvents);
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

        private async Task OnCancelEventAsync(MobEvent mobEvent)
        {
            var currentUser = App.CurrentUser;
            //TODO: dialog confirmation?
            if (currentUser != null)
            {
                var cancelEvent = new CancelEvent
                {
                    EventId = mobEvent.Id,
                    CancellationReason = string.Empty //TODO: UI for this?
                };
                _isLoading = true;
                await MobEventManager.DeleteEventAsync(cancelEvent);
                await GetMyEventsAsync();
                _isLoading = false;
            }
        }

        private async Task OnEditAsync(MobEvent mobEvent)
            => Navigator.NavigateTo(string.Format(Routes.EditEvent, mobEvent.Id));
    }
}
