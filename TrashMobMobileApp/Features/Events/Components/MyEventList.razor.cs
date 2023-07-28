namespace TrashMobMobileApp.Features.Events.Components
{
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Enums;
    using TrashMobMobileApp.Features.Map;
    using TrashMobMobileApp.Shared;
    using TrashMobMobileApp.StateContainers;

    public partial class MyEventList
    {
        private List<Event> _myEvents = new();
        private List<Event> _myEventsStatic = new();
        private bool _isLoading;
        private string _eventSearchText;
        private EventActionGroup _currentSelectedChip = EventActionGroup.NONE;
        private User _user;

        [Inject]
        public IMapRestService MapRestService { get; set; }

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Inject]
        public IWaiverManager WaiverManager { get; set; }

        [Inject]
        public UserStateInformation StateInformation { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _user = App.CurrentUser;
            _currentSelectedChip = EventActionGroup.OWNER;
            EventContainer.UserEventInteractionAction += HandleEventInteractionOutcome;
            await ReInitializeAsync();
        }

        private async Task ReInitializeAsync()
        {
            _isLoading = true;
            _myEventsStatic = (await MobEventManager.GetUserEventsAsync(_user.Id, StateInformation.ShowFutureEvents)).OrderByDescending(x => x.EventDate).ToList();
            _myEvents = _myEventsStatic;
            _isLoading = false;
        }

        private void HandleEventInteractionOutcome(UserEventInteraction interaction)
        {
            switch (interaction)
            {
                case UserEventInteraction.CREATED_EVENT:
                    Snackbar.Add("Event created!", Severity.Success);
                    Navigator.NavigateTo(Routes.Events, forceLoad: true);
                    break;
                case UserEventInteraction.SUBMITTED_EVENT:
                    Snackbar.Add("Event summary submitted!", Severity.Success);
                    Navigator.NavigateTo(Routes.PickupLocations, forceLoad: true);
                    break;
                case UserEventInteraction.ADDPICKUPLOCATIONS_EVENT:
                    Snackbar.Add("Pickup Location submitted!", Severity.Success);
                    Navigator.NavigateTo(Routes.Events, forceLoad: true);
                    break;
                case UserEventInteraction.EDITED_EVENT:
                    Snackbar.Add("Event updated!", Severity.Success);
                    Navigator.NavigateTo(Routes.Events, forceLoad: true);
                    break;
                case UserEventInteraction.CANCELLED_EVENT:
                    Snackbar.Add("Event cancelled!", Severity.Success);
                    Navigator.NavigateTo(Routes.Events, forceLoad: true);
                    break;
                case UserEventInteraction.NONE:
                default:
                    break;
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
            Navigator.NavigateTo(string.Format(Routes.ViewEvent, mobEvent.Id));
        }

        private void OnCompleteEvent(Event mobEvent)
            => Navigator.NavigateTo(string.Format(Routes.CompleteEvent, mobEvent.Id.ToString(), false));

        private async Task OnShowFutureEventsChangedAsync(bool val)
        {
            StateInformation.ShowFutureEvents = val;
            await ReInitializeAsync();
        }

        private void OnViewEventSummary(Event mobEvent)
            => Navigator.NavigateTo(string.Format(Routes.CompleteEvent, mobEvent.Id.ToString(), true));

        private void OnCancelEvent(Event mobEvent) 
            => Navigator.NavigateTo(string.Format(Routes.CancelEvent, mobEvent.Id.ToString()));

        private void OnEdit(Event mobEvent)
            => Navigator.NavigateTo(string.Format(Routes.EditEvent, mobEvent.Id));

        private async void OnViewMapAllEvents(IEnumerable<Event> mobEvents)
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new MauiMapPageMultipleEvent(MobEventManager, WaiverManager, StateInformation, mobEvents));
        }

        private async void OnViewMap(Event mobEvent)
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new MauiMapPageSingleEvent(MapRestService, mobEvent, false));
            await ReInitializeAsync();
        }

        private async Task OnAttendingEventsFilterAsync()
        {
            _myEvents.Clear();
            _currentSelectedChip = EventActionGroup.ATTENDING;
            var attendingEvents = await MobEventManager.GetEventsUserIsAttending(_user.Id);
            _myEvents = attendingEvents.ToList() ?? new List<Event>();
        }

        private async Task OnOwningEventsFilterAsync()
        {
            _myEvents.Clear();
            _currentSelectedChip = EventActionGroup.OWNER;
            var owningEvents = await MobEventManager.GetUserEventsAsync(_user.Id, false);
            _myEvents = owningEvents.ToList() ?? new List<Event>();
        }

        private async Task OnPastEventsFilterAsync()
        {
            _myEvents.Clear();
            _currentSelectedChip = EventActionGroup.PAST_EVENTS;
            var ownedEvents = (await MobEventManager.GetUserEventsAsync(_user.Id, false))?.Where(mobEvent => mobEvent.EventDate < DateTimeOffset.UtcNow);
            var attendingEvents = (await MobEventManager.GetEventsUserIsAttending(_user.Id))?.Where(mobEvent => mobEvent.EventDate < DateTimeOffset.UtcNow);
            
            if (ownedEvents != null && ownedEvents.Any())
            {
                _myEvents.AddRange(ownedEvents);
            }

            if (attendingEvents != null && attendingEvents.Any())
            {
                _myEvents.AddRange(attendingEvents);
            }
        }

        private bool DoesUserOwnEvent(Event mobEvent)
        {
            if (_user.Id == mobEvent.CreatedByUserId)
            {
                return true;
            }

            return false;
        }

        private bool IsPastEvent(Event mobEvent)
        {
            return mobEvent.EventDate.LocalDateTime <= DateTimeOffset.UtcNow.LocalDateTime;
        }

        private bool IsEventCompleted(Event mobEvent)
        {
            return mobEvent.EventStatusId == 4; 
        }

        private EventActionType DetermineUserCardAction(Event mobEvent)
        {
            if (DoesUserOwnEvent(mobEvent) && IsPastEvent(mobEvent) && !IsEventCompleted(mobEvent))
            {
                return EventActionType.SUBMIT_SUMMARY;
            }
            else if (DoesUserOwnEvent(mobEvent) && !IsEventCompleted(mobEvent))
            {
                return EventActionType.MANAGE;
            }
            else if (IsEventCompleted(mobEvent))
            {
                return EventActionType.VIEW_SUMMARY;
            }

            return EventActionType.NONE;
        }
    }
}
