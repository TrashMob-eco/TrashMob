namespace TrashMobMobileApp.Features.Events.Pages
{
    using CommunityToolkit.Maui.Views;
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Extensions;
    using TrashMobMobileApp.Features.Map;
    using TrashMobMobileApp.Shared;

    public partial class EditEvent
    {
        private List<EventType> _eventTypes = new();
        private EventType _selectedEventType;
        private MudForm _editEventForm;
        private bool _isLoading;
        private bool _success;
        private string[] _errors;
        private Event _event = new();
        private DateTime? _eventDate
            = DateTime.Now;
        private TimeSpan? _eventTime
            = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        [Inject]
        public IEventTypeRestService EventTypesService { get; set; }

        [Inject]
        public IMapRestService MapRestService { get; set; }

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Parameter]
        public string EventId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TitleContainer.Title = "Edit Event";
            await GetEventTypesAsync();
            await GetAndSetEventInfoAsync();

        }

        private async Task GetEventTypesAsync()
        {
            _isLoading = true;
            _eventTypes = (await EventTypesService.GetEventTypesAsync()).ToList();
            _selectedEventType = _eventTypes?.FirstOrDefault();
            _isLoading = false;
        }

        private async Task GetAndSetEventInfoAsync()
        {
            _isLoading = true;
            _event = await MobEventManager.GetEventAsync(Guid.Parse(EventId));
            _isLoading = false;
            if (_event != null)
            {
                _eventDate = TimeZoneInfo.ConvertTime(_event.EventDate, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.StandardName)).DateTime;
                _eventTime = new TimeSpan(_event.EventDate.Hour, _event.EventDate.Minute, 0);
                _selectedEventType = _eventTypes.FirstOrDefault(item => item.Id == _event.EventTypeId);
            }
        }

        private async void OpenMap()
        {
            var result = await App.Current.MainPage.ShowPopupAsync(new EditMapPopup(MapRestService, _event));

            if (result != null)
            {
                _event = result as Event;
                StateHasChanged();
            }
        }

        private async Task OnSaveAsync()
        {
            try
            {
                await _editEventForm?.Validate();
                if (_success)
                {
                    _event.EventDate = new DateTime(_eventDate.Value.Year, _eventDate.Value.Month, _eventDate.Value.Day,
                        _eventTime.Value.Hours, _eventTime.Value.Minutes, default);
                    _event.CreatedByUserId = App.CurrentUser.Id;
                    _event.LastUpdatedByUserId = App.CurrentUser.Id;
                    _event.LastUpdatedDate = DateTime.Now;
                    _event.EventTypeId = _selectedEventType.Id;
                    _isLoading = true;
                    var eventAdd = await MobEventManager.UpdateEventAsync(_event);
                    _isLoading = false;
                }
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
                EventContainer.UserEventInteractionAction.Invoke(Enums.UserEventInteraction.EDITED_EVENT);
            }
        }

        private void OnCancel() => Navigator.NavigateTo(Routes.Events);
    }
}
