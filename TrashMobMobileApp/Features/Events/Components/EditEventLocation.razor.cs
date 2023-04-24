namespace TrashMobMobileApp.Features.Events.Components
{
    using CommunityToolkit.Maui.Views;
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Extensions;
    using TrashMobMobileApp.Features.Map;

    public partial class EditEventLocation
    {
        private bool _isLoading = false;
        private Event _event;
        private bool _success;
        private string[] _errors;

        [Inject]
        public IMapRestService MapRestService { get; set; }

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Parameter]
        public string EventId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            _event = await MobEventManager.GetEventAsync(Guid.Parse(EventId));
            _isLoading = false;
        }

        private async void OpenMap()
        {
            var result = await App.Current.MainPage.ShowPopupAsync(new EditMapPopup(MapRestService, _event));

            if (result != null)
            {
                _event = result as Event;
                StateHasChanged();
            }
        }

        private async Task OnSaveAsync()
        {
            try
            {
                var eventUpdate = await MobEventManager.UpdateEventAsync(_event);
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
            }
        }
    }
}
