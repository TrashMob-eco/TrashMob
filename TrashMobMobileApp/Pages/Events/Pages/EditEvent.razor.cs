using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;
using TrashMobMobileApp.Shared;

namespace TrashMobMobileApp.Pages.Events.Pages
{
    public partial class EditEvent
    {
        private List<EventType> _eventTypes = new();
        private EventType _selectedEventType;
        private MudForm _editEventForm;
        private bool _isLoading;
        private bool _success;
        private string[] _errors;
        private MobEvent _event = new();
        private DateTime? _eventDate
            = DateTime.Now;
        private TimeSpan? _eventTime
            = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        private int _zip;

        [Inject]
        public IEventTypeRestService EventTypesService { get; set; }

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
                _eventDate = new DateTime(_event.EventDate.Year, _event.EventDate.Month, _event.EventDate.Day);
                _eventTime = new TimeSpan(_event.EventDate.Hour, _event.EventDate.Minute, 0);
                _zip = Convert.ToInt32(_event.PostalCode);
                _selectedEventType = _eventTypes.FirstOrDefault(item => item.Id == _event.EventTypeId);
            }
        }

        private async Task OnAddAsync()
        {
            var isValid = _editEventForm?.IsValid;
            if (isValid.HasValue && isValid.Value)
            {
                _event.EventDate = new DateTime(_eventDate.Value.Year, _eventDate.Value.Month, _eventDate.Value.Day,
                    _eventTime.Value.Hours, _eventTime.Value.Minutes, default);
                _event.PostalCode = _zip.ToString();
                _event.CreatedByUserId = App.CurrentUser.Id;
                _event.LastUpdatedByUserId = App.CurrentUser.Id;
                _event.LastUpdatedDate = DateTime.Now;
                _event.EventTypeId = _selectedEventType.Id;
                _event.EventStatusId = 1;
                _isLoading = true;
                var eventAdd = await MobEventManager.AddEventAsync(_event);
                _isLoading = false;
                if (eventAdd != null)
                {
                    Navigator.NavigateTo(Routes.Events);
                }
            }
            else
            {
                _editEventForm?.Validate();
            }
        }

        private void OnCancel() => Navigator.NavigateTo(Routes.Events);
    }
}
