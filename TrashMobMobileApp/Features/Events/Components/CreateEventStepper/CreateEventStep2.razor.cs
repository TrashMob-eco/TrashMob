using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMob.Models;

namespace TrashMobMobileApp.Features.Events.Components
{
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

        protected override void OnInitialized()
        {
            TitleContainer.Title = "Create Event (2/6)";
        }

        private async Task OnStepFinishedAsync()
        {
            await _form.Validate();
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
