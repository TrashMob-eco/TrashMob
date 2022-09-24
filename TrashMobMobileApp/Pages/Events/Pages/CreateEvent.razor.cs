using Microsoft.AspNetCore.Components;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;
using TrashMobMobileApp.Shared;

namespace TrashMobMobileApp.Pages.Events.Pages
{
    public partial class CreateEvent
    {
        private MobEvent _event = new();
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
