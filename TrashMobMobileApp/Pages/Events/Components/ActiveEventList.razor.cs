using Microsoft.AspNetCore.Components;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;

namespace TrashMobMobileApp.Pages.Events.Components
{
    public partial class ActiveEventList
    {
        private List<MobEvent> _mobEvents = new();
        private bool _isLoading;
        private MobEvent _selectedEvent;
        private bool _isViewOpen;

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            _mobEvents = (await MobEventManager.GetActiveEventsAsync()).ToList();
            var randomEvents = new List<MobEvent>
            {
                new MobEvent
                {
                    Name = "Event Random 1",
                    Description = "Event Description 1"
                },
                new MobEvent
                {
                    Name = "Event Random 2",
                    Description = "Event Description 2"
                },
                new MobEvent
                {
                    Name = "Event Random 3",
                    Description = "Event Description 3"
                }
            };
            _mobEvents.AddRange(randomEvents);
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
            }
        }
    }
}
