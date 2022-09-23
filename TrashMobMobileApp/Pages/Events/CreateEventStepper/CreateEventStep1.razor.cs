using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;

namespace TrashMobMobileApp.Pages.Events.CreateEventStepper
{
    public partial class CreateEventStep1
    {
        private MudForm _step1Form;
        private bool _success;
        private string[] _errors;
        private bool _isLoading;
        private List<EventType> _eventTypes = new();
        private EventType _selectedEventType;

        [Inject]
        public IEventTypeRestService EventTypesService { get; set; }

        [Parameter]
        public MobEvent Event { get; set; }

        [Parameter]
        public EventCallback<MobEvent> EventChanged { get; set; }

        [Parameter]
        public EventCallback OnStepFinished { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TitleContainer.Title = "Create Event (1/6)";
            await GetEventTypesAsync();
            _selectedEventType = _eventTypes.FirstOrDefault();
        }

        private async Task GetEventTypesAsync()
        {
            _isLoading = true;
            _eventTypes = (await EventTypesService.GetEventTypesAsync()).ToList();
            _isLoading = false;
        }

        private async Task OnStepFinishedAsync()
        {
            Event.EventTypeId = _selectedEventType.Id;
            await _step1Form.Validate();
            if (_success)
            {
                if (OnStepFinished.HasDelegate)
                {
                    await OnStepFinished.InvokeAsync();
                }
            }
        }
    }
}
