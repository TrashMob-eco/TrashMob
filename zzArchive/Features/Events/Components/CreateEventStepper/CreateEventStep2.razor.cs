namespace TrashMobMobileApp.Features.Events.Components
{
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMob.Models;

    public partial class CreateEventStep2
    {
        private MudForm _form;
        private bool _success;
        private string[] _errors;
        private DateTime? _eventDate;
        private TimeSpan? _eventTime;

        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public EventCallback<Event> EventChanged { get; set; }

        [Parameter]
        public EventCallback OnStepFinished { get; set; }

        private bool publicEventError = false;
        private string publicEventErrorText = string.Empty;

        protected override void OnInitialized()
        {
            TitleContainer.Title = "Create Event (2/5)";
        }

        private bool ValidatePublicPrivate()
        {
            var eventDate = new DateTime(_eventDate.Value.Year, _eventDate.Value.Month, _eventDate.Value.Day,
    _eventTime.Value.Hours, _eventTime.Value.Minutes, 0);

            if (eventDate <= DateTimeOffset.UtcNow && Event.IsEventPublic)
            {
                publicEventError = true;
                publicEventErrorText = "Public Events cannot be in the past";
                return false;
            }

            publicEventError = false;
            publicEventErrorText = "";

            return true;
        }

        private async Task OnStepFinishedAsync()
        {
            await _form.Validate();

            if (_success)
            {
                _success = ValidatePublicPrivate();

                if (_success)
                {
                    Event.EventDate = new DateTime(_eventDate.Value.Year, _eventDate.Value.Month, _eventDate.Value.Day,
                        _eventTime.Value.Hours, _eventTime.Value.Minutes, 0);

                    if (OnStepFinished.HasDelegate)
                    {
                        await OnStepFinished.InvokeAsync();
                    }
                }
            }
        }
    }
}
