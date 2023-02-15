namespace TrashMobMobileApp.Features.Events.Components
{
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Extensions;
    using TrashMobMobileApp.Features.Map;
    using TrashMobMobileApp.Shared;

    public partial class ActiveEventList
    {
        private List<Event> _mobEventsStatic = new();
        private List<Event> _mobEvents = new();
        private List<Guid> _userAttendingEventIds = new();
        private bool _isLoading;
        private string _eventSearchText;
        private User _user;

        [Inject]
        public IMapRestService MapRestService { get; set; }

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _user = App.CurrentUser;
            _isLoading = true;
            _mobEventsStatic = (await MobEventManager.GetActiveEventsAsync()).ToList();
            _userAttendingEventIds = (await MobEventManager.GetEventsUserIsAttending(_user.Id)).Select(x => x.Id).ToList();
            _mobEvents = _mobEventsStatic;
            _isLoading = false;
        }

        private void OnViewEventDetails(Event mobEvent)
        {
            Navigator.NavigateTo(string.Format(Routes.ViewEvent, mobEvent.Id));
        }

        private void OnViewMapAllEvents(IEnumerable<Event> mobEvents)
        {
            App.Current.MainPage.Navigation.PushModalAsync(new MauiMapPageMultipleEvent(mobEvents));
        }

        private void OnViewMap(Event mobEvent)
        {
            var isEventOwner = _user.Id == mobEvent.CreatedByUserId;
            
            App.Current.MainPage.Navigation.PushModalAsync(new MauiMapPageSingleEvent(MapRestService, mobEvent, isEventOwner));
        }

        private async Task OnRegisterAsync(Event mobEvent)
        {
            try
            {
                var attendee = new EventAttendee
                {
                    UserId = _user.Id,
                    EventId = mobEvent.Id
                };

                _isLoading = true;
                await MobEventManager.AddEventAttendeeAsync(attendee);
                _isLoading = false;
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
                Snackbar.Add($"Registered!", Severity.Success);
            }
        }

        private async Task OnUnregisterAsync(Event mobEvent)
        {
            try
            {
                var attendee = new EventAttendee
                {
                    UserId = _user.Id,
                    EventId = mobEvent.Id
                };

                _isLoading = true;
                await MobEventManager.RemoveEventAttendeeAsync(attendee);
                _isLoading = false;
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
                Snackbar.Add($"Unregistered!", Severity.Success);
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
