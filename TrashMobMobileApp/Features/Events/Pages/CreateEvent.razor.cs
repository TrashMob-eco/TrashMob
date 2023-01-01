using Microsoft.AspNetCore.Components;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Shared;
using TrashMob.Models;

namespace TrashMobMobileApp.Features.Events.Pages
{
    public partial class CreateEvent
    {
        private Event _event = new();
        private EventStep _step;

        [Inject]
        public IEventTypeRestService EventTypesService { get; set; }

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        protected override void OnInitialized()
        {
            TitleContainer.Title = "Create Event";
            _step = EventStep.STEP_1;
        }

        private void OnNextStep(EventStep _nextStep) => _step = _nextStep;

        private void OnFinished() => Navigator.NavigateTo(Routes.Events);
    }
}
