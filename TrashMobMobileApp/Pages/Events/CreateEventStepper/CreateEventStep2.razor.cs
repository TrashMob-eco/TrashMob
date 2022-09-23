using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;

namespace TrashMobMobileApp.Pages.Events.CreateEventStepper
{
    public partial class CreateEventStep2
    {
        private MudForm _form;
        private bool _success;
        private string[] _errors;
        private DateTime? _eventDate;
        private TimeSpan? _eventTime;

        [Parameter]
        public MobEvent Event { get; set; }

        [Parameter]
        public EventCallback<MobEvent> EventChanged { get; set; }

        [Parameter]
        public EventCallback OnStepFinished { get; set; }

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
