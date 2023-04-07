namespace TrashMobMobileApp.Features.Events.Pages
{
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Extensions;
    using TrashMobMobileApp.Features.Pickups;
    using TrashMobMobileApp.Shared;

    public partial class CompleteEvent
    {
        MudForm _completeEventForm;
        private bool _success;
        private string[] _errors;
        private Event _event;
        private EventSummary _eventSummary = new();
        private bool _isLoading;

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Inject]
        public IMapRestService MapRestService { get; set; }

        [Inject]
        public IPickupLocationRestService PickupLocationRestService { get; set; }

        [Parameter]
        public string EventId { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TitleContainer.Title = IsReadOnly ? "Summary" : "Complete Event";
            _isLoading = true;
            _event = await MobEventManager.GetEventAsync(Guid.Parse(EventId));
            _eventSummary = (await MobEventManager.GetEventSummaryAsync(Guid.Parse(EventId))) ?? new();
            _isLoading = false;
        }

        private void AddPickupLocation()
        {
            App.Current.MainPage.Navigation.PushModalAsync(new AddPickupLocation(MapRestService, PickupLocationRestService, _event.Id));
        }

        private async Task OnDoActionAsync()
        {
            if (IsReadOnly)
            {
                Navigator.NavigateTo(Routes.Events, forceLoad: true);
            }
            else
            {
                await OnSubmitAsync();
            }
        }

        private async Task OnSubmitAsync()
        {
            try
            {
                await _completeEventForm?.Validate();
                if (_success)
                {
                    _eventSummary.EventId = _event.Id;
                    _eventSummary.CreatedByUserId = App.CurrentUser.Id;
                    _eventSummary.CreatedDate = DateTimeOffset.UtcNow;
                    _eventSummary.LastUpdatedByUserId = App.CurrentUser.Id;
                    _event.EventStatusId = 4;
                    _event.EventSummary = _eventSummary;
                    _isLoading = true;
                    await MobEventManager.UpdateEventAsync(_event);
                    _isLoading = false;
                }
            }
            catch (Exception ex)
            {
                if (ex.IsClosedStreamException())
                {
                    return;
                }
            }
            finally
            {
                _isLoading = false;
                EventContainer.UserEventInteractionAction.Invoke(Enums.UserEventInteraction.SUBMITTED_EVENT);
            }
        }
    }
}
