namespace TrashMobMobileApp.Features.Events.Pages
{
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMobMobileApp.Data;
    using TrashMob.Models;
    using TrashMobMobileApp.Extensions;

    public partial class CancelEvent
    {
        private bool _isLoading;
        private Event _event;
        private MudForm _cancelEventForm;
        private bool _success;
        private string[] _errors;
#nullable enable
        private string? _cancelReason;
#nullable disable

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Parameter]
        public string EventId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TitleContainer.Title = "Cancel Event";
            _isLoading = true;
            _event = await MobEventManager.GetEventAsync(Guid.Parse(EventId));
            _isLoading = false;
        }

        private async Task OnCancelEventAsync()
        {
            try
            {
                await _cancelEventForm?.Validate();
                if (_success)
                {
                    _isLoading = true;
                    var cancelEvent = new Models.EventCancellationRequest
                    {
                        EventId = _event.Id,
                        CancellationReason = _cancelReason
                    };

                    _isLoading = true;
                    await MobEventManager.DeleteEventAsync(cancelEvent);
                    _isLoading = false;
                }
            }
            catch(Exception ex)
            {
                if (ex.IsClosedStreamException())
                {
                    return;
                }
            }
            finally
            {
                _isLoading = false;
                EventContainer.UserEventInteractionAction.Invoke(Enums.UserEventInteraction.CANCELLED_EVENT);
            }
        }
    }
}
