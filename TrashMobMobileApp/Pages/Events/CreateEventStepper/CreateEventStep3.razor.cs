using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMob.Models;

namespace TrashMobMobileApp.Pages.Events.CreateEventStepper
{
    public partial class CreateEventStep3
    {
        private MudForm _form;
        private bool _success;
        private string[] _errors;
        private int _postal;

        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public EventCallback<Event> EventChanged { get; set; }

        [Parameter]
        public EventCallback OnStepFinished { get; set; }

        protected override void OnInitialized()
        {
            TitleContainer.Title = "Create Event (3/6)";
        }

        private async Task OnStepFinishedAsync()
        {
            await _form.Validate();
            if (_success)
            {
                Event.PostalCode = _postal.ToString();
                Event.Country = "USA";
                if (OnStepFinished.HasDelegate)
                {
                    await OnStepFinished.InvokeAsync();
                }
            }
        }
    }
}
