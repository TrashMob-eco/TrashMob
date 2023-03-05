namespace TrashMobMobileApp.Features.Events.Components
{
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Shared;

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

        [Inject]
        public IWaiverManager WaiverManager { get; set; }

        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public EventCallback<Event> EventChanged { get; set; }

        [Parameter]
        public EventCallback OnStepFinished { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var hasSignedWaiver = await WaiverManager.HasUserSignedTrashMobWaiverAsync();

            if (!hasSignedWaiver)
            {
                Navigator.NavigateTo(Routes.Waiver);
            }

            TitleContainer.Title = "Create Event (1/5)";
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
