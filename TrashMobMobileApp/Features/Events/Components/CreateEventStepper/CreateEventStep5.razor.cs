namespace TrashMobMobileApp.Features.Events.Components
{
    using CommunityToolkit.Maui.Views;
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Features.Map;

    public partial class CreateEventStep5
    {
        private MudForm _form;
        private bool _success;
        private string[] _errors;
        private List<EventType> _eventTypes = new();
        private EventType _selectedEventType;
        private DateTime? _eventDate;
        private TimeSpan? _eventTime;

        [Inject]
        public IEventTypeRestService EventTypesService { get; set; }

        [Inject]
        public IMapRestService MapRestService { get; set; }

        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public EventCallback<Event> EventChanged { get; set; }

        [Parameter]
        public EventCallback OnStepFinished { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TitleContainer.Title = "Create Event (4/5)";
            await GetEventTypesAsync();
            _selectedEventType = _eventTypes.Find(item => item.Id == Event.EventTypeId);
            _eventDate = TimeZoneInfo.ConvertTime(Event.EventDate, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.StandardName)).DateTime;
            _eventTime = new TimeSpan(Event.EventDate.Hour, Event.EventDate.Minute, 0);
        }

        private async Task GetEventTypesAsync()
        {
            _eventTypes = (await EventTypesService.GetEventTypesAsync()).ToList();
        }

        private async void OpenMap()
        {
            var result = await App.Current.MainPage.ShowPopupAsync(new EditMapPopup(MapRestService, Event));

            if (result != null)
            {
                Event = result as Event;
                StateHasChanged();
            }
        }

        private async Task OnStepFinishedAsync()
        {
            await _form.Validate();
            if (_success)
            {
                Event.EventTypeId = _selectedEventType.Id;
                Event.EventDate = new DateTimeOffset(new DateTime(_eventDate.Value.Year, _eventDate.Value.Month, _eventDate.Value.Day,
                    _eventTime.Value.Hours, _eventTime.Value.Minutes, 0));
                if (OnStepFinished.HasDelegate)
                {
                    await OnStepFinished.InvokeAsync();
                }
            }
        }
    }
}
