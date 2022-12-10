using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMob.Models;
using TrashMobMobileApp.Data;

namespace TrashMobMobileApp.Pages.Events.Components
{
    public partial class EventDetails
    {
        private EventSummary _summary;
        private Typo BodyText => Typo.body2;

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public EventCallback<bool> OpenChanged { get; set; }

        [Parameter]
        public Anchor Anchor { get; set; } = Anchor.End;

        [Parameter]
        public Event Event { get; set; }

        private async Task OnEventSummaryExpandedAsync(bool expanded)
        {
            if (expanded)
            {
                _summary = await MobEventManager.GetEventSummaryAsync(Event.Id);
            }
        }
    }
}
