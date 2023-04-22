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
        private bool isNewSummary = true;
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
             
            var eventSummary = (await MobEventManager.GetEventSummaryAsync(Guid.Parse(EventId)));
            if (eventSummary != null)
            {
                _eventSummary = eventSummary;
                isNewSummary = false;
            }
            else
            {
                _eventSummary.EventId = Guid.Parse(EventId);
                _eventSummary.CreatedByUserId = App.CurrentUser.Id;
            }

            _isLoading = false;
        }

        private void AddPickupLocation()
        {
            App.Current.MainPage.Navigation.PushModalAsync(new AddPickupLocation(MapRestService, PickupLocationRestService, _event.Id.ToString()));
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
                    _isLoading = true;

                    if (isNewSummary)
                    {
                        await MobEventManager.AddEventSummaryAsync(_eventSummary);
                    }
                    else
                    {
                        await MobEventManager.UpdateEventSummaryAsync(_eventSummary);
                    }

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
                EventContainer.EventId = EventId;
                EventContainer.UserEventInteractionAction.Invoke(Enums.UserEventInteraction.SUBMITTED_EVENT);
            }
        }
    }
}
