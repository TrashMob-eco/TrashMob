using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMob.Models;
using TrashMobMobileApp.Data;

namespace TrashMobMobileApp.Features.Events.Components
{
    public partial class CreateEventStep5
    {
        private MudForm _form;
        private bool _success;
        private string[] _errors;
        private bool _isLoading;
        private List<EventType> _eventTypes = new();
        private EventType _selectedEventType;
        private DateTime? _eventDate;
        private TimeSpan? _eventTime;
        private int _postal;

        [Inject]
        public IEventTypeRestService EventTypesService { get; set; }

        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public EventCallback<Event> EventChanged { get; set; }

        [Parameter]
        public EventCallback OnStepFinished { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TitleContainer.Title = "Create Event (5/6)";
            await GetEventTypesAsync();
            _selectedEventType = _eventTypes.Find(item => item.Id == Event.EventTypeId);
            //TODO: get user's timezone
            _eventDate = TimeZoneInfo.ConvertTime(Event.EventDate, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")).DateTime;
            _eventTime = new TimeSpan(Event.EventDate.Hour, Event.EventDate.Minute, 0);
            _postal = Convert.ToInt32(Event.PostalCode);
        }

        private async Task GetEventTypesAsync()
        {
            _isLoading = true;
            _eventTypes = (await EventTypesService.GetEventTypesAsync()).ToList();
            _isLoading = false;
        }

        private async Task OnStepFinishedAsync()
        {
            await _form.Validate();
            if (_success)
            {
                Event.EventTypeId = _selectedEventType.Id;
                Event.EventDate = new DateTimeOffset(new DateTime(_eventDate.Value.Year, _eventDate.Value.Month, _eventDate.Value.Day,
                    _eventTime.Value.Hours, _eventTime.Value.Minutes, 0));
                Event.PostalCode = _postal.ToString();
                if (OnStepFinished.HasDelegate)
                {
                    await OnStepFinished.InvokeAsync();
                }
            }
        }
    }
}
