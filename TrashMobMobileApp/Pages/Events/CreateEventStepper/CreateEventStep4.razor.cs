using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMobMobileApp.Models;

namespace TrashMobMobileApp.Pages.Events.CreateEventStepper
{
    public partial class CreateEventStep4
    {
        private MudForm _form;
        private bool _success;
        private string[] _errors;

        [Parameter]
        public MobEvent Event { get; set; }

        [Parameter]
        public EventCallback<MobEvent> EventChanged { get; set; }

        [Parameter]
        public EventCallback OnStepFinished { get; set; }

        protected override void OnInitialized()
        {
            TitleContainer.Title = "Create Event (4/6)";
        }

        private async Task OnStepFinishedAsync()
        {
            await _form.Validate();
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
