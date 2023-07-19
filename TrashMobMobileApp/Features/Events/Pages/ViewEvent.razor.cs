namespace TrashMobMobileApp.Features.Events.Pages
{
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Extensions;
    using TrashMobMobileApp.Shared;

    public partial class ViewEvent
    {
        private bool _isLoading;
        private Event _event = new();
        private EventType _eventType = new(); 
        private User _user;
        private List<Guid> _userAttendingEventIds = new();

        [Parameter]
        public string EventId { get; set; }

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Inject]
        public IWaiverManager WaiverManager { get; set; }

        [Inject]
        public IUserManager UserManager { get; set; }

        [Inject]
        public IEventTypeRestService EventTypeManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _isLoading = true;
            var user = await UserManager.GetUserAsync(App.CurrentUser.Id.ToString());
            App.CurrentUser = user;
            _user = user;
            _userAttendingEventIds = (await MobEventManager.GetEventsUserIsAttending(_user.Id)).Select(x => x.Id).ToList();
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

        private async Task OnRegisterAsync(Event mobEvent)
        {
            try
            {
                var hasSignedWaiver = await WaiverManager.HasUserSignedTrashMobWaiverAsync();

                if (!hasSignedWaiver)
                {
                    Navigator.NavigateTo(Routes.Waiver);
                }

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
    }
}
