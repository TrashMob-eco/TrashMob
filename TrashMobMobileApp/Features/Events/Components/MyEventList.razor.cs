using Microsoft.AspNetCore.Components;
using TrashMob.Models;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Enums;
using TrashMobMobileApp.Shared;
using TrashMobMobileApp.StateContainers;

namespace TrashMobMobileApp.Features.Events.Components
{
    public partial class MyEventList
    {
        private List<Event> _myEvents = new();
        private List<Event> _myEventsStatic = new();
        private bool _isLoading;
        private string _eventSearchText;
        private EventActionGroup _currentSelectedChip = EventActionGroup.NONE;
        private bool _eventSummarySubmitted;

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Inject]
        public UserStateInformation StateInformation { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetMyEventsAsync();
        }

        private async Task GetMyEventsAsync()
        {
            var currentUser = App.CurrentUser;
            if (currentUser != null)
            {
                _isLoading = true;
                _myEventsStatic = (await MobEventManager.GetUserEventsAsync(currentUser.Id, StateInformation.ShowFutureEvents)).ToList();
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

        private void OnViewEventDetails(Event mobEvent)
        {
            var uri = string.Format(Routes.EditEvent, mobEvent.Id, true);
            Navigator.NavigateTo(uri);
        }

        private void OnCompleteEvent(Event mobEvent)
            => Navigator.NavigateTo(string.Format(Routes.CompleteEvent, mobEvent.Id.ToString(), false));

        private async Task OnShowFutureEventsChangedAsync(bool val)
        {
            StateInformation.ShowFutureEvents = val;
            await GetMyEventsAsync();
        }

        private void OnViewEventSummary(Event mobEvent)
            => Navigator.NavigateTo(string.Format(Routes.CompleteEvent, mobEvent.Id.ToString(), true));

        private void OnCancelEvent(Event mobEvent) 
            => Navigator.NavigateTo(string.Format(Routes.CancelEvent, mobEvent.Id.ToString()));

        private void OnEdit(Event mobEvent)
            => Navigator.NavigateTo(string.Format(Routes.EditEvent, mobEvent.Id, false));

        private async Task OnAttendingEventsFilterAsync()
        {
            _currentSelectedChip = EventActionGroup.ATTENDING;
            var currentUser = App.CurrentUser;
            var attendingEvents = await MobEventManager.GetEventsUserIsAttending(currentUser.Id);
            _myEvents = attendingEvents.ToList() ?? new List<Event>();
        }

        private async Task OnOwningEventsFilterAsync()
        {
            _currentSelectedChip = EventActionGroup.OWNER;
            var currentUser = App.CurrentUser;
            var owningEvents = await MobEventManager.GetUserEventsAsync(currentUser.Id, false);
            _myEvents = owningEvents.ToList() ?? new List<Event>();
        }

        private void OnPastEventsFilter()
        {
            _currentSelectedChip = EventActionGroup.PAST_EVENTS;
            _myEvents = _myEvents.Where(mobEvent => mobEvent.EventDate <= DateTimeOffset.UtcNow).ToList() ?? new List<Event>();
        }

        private bool DoesUserOwnEvent(Event mobEvent)
        {
            var currentUser = App.CurrentUser;
            if (currentUser.Id == mobEvent.CreatedByUserId)
            {
                return true;
            }

            return false;
        }

        private bool IsPastEvent(Event mobEvent)
        {
            return mobEvent.EventDate <= DateTimeOffset.UtcNow;
        }

        private async Task<bool> IsEventSummarySubmitted(Event mobEvent)
        {
            var eventSummary = await MobEventManager.GetEventSummaryAsync(mobEvent.Id);
            if (eventSummary != null)
            {
                return true;
            }

            return false;
        }

        private EventActionType DetermineUserCardAction(Event mobEvent)
        {
            if (DoesUserOwnEvent(mobEvent) && IsPastEvent(mobEvent))
            {
                return EventActionType.SUBMIT_SUMMARY;
            }
            else if (DoesUserOwnEvent(mobEvent))
            {
                return EventActionType.MANAGE;
            }

            return EventActionType.VIEW_SUMMARY;
        }
    }
}
