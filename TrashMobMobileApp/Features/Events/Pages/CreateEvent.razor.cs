namespace TrashMobMobileApp.Features.Events.Pages
{
    using Microsoft.AspNetCore.Components;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Shared;
    using TrashMob.Models;
    using CommunityToolkit.Maui.Views;
    using TrashMobMobileApp.Features.Map;

    public partial class CreateEvent
    {
        private Event _event = new() { IsEventPublic = true };
        private EventStep _step;

        [Inject]
        public IEventTypeRestService EventTypesService { get; set; }

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Inject]
        public IMapRestService MapRestService { get; set; }

        protected override void OnInitialized()
        {
            TitleContainer.Title = "Create Event";
            _step = EventStep.STEP_1;
        }

        private void OnNextStep(EventStep _nextStep) => _step = _nextStep;

        private async void OpenMap()
        {
            var result = await App.Current.MainPage.ShowPopupAsync(new EditMapPopup(MapRestService, _event));
            if (result != null)
            { 
                _event = result as Event;
                _step = EventStep.STEP_5;
                StateHasChanged();
            }
        }

        private void OnFinished() => Navigator.NavigateTo(Routes.Events, forceLoad: true);
    }
}
