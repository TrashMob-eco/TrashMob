namespace TrashMobMobileApp.Features.Events.Components
{
    using Microsoft.AspNetCore.Components;
    using TrashMobMobileApp.Data;
    using TrashMob.Models;
    using TrashMobMobileApp.Extensions;
    
    public partial class CreateEventStep6
    {
        [Parameter]
        public Event Event { get; set; }

        [Parameter]
        public EventCallback<Event> EventChanged { get; set; }

        [Parameter]
        public EventCallback OnStepFinished { get; set; }

        protected override Task OnInitializedAsync()
        {
            TitleContainer.Title = "Add Partners to Event";
            return Task.CompletedTask;
        }

        private async Task OnStepFinishedAsync()
        {
            if (OnStepFinished.HasDelegate)
            {
                await OnStepFinished.InvokeAsync();
            }
        }
    }
}
