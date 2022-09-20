using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;

namespace TrashMobMobileApp.Pages.Events.Components
{
    public partial class EventDetails
    {
        private EventSummary? _summary;
        public string GetFormattedAddress
            => Event == null ? string.Empty : string.Concat(Event?.StreetAddress, ", ", Event?.City, ", ", Event?.Region, ", ", Event?.Country);
        public string GetFormattedDuration
            => Event == null ? string.Empty : string.Concat(Event?.DurationHours, " hr ", Event?.DurationMinutes, " mins");
        public string PublicEvent => (Event?.IsEventPublic).HasValue && (Event?.IsEventPublic).Value ? "Yes" : "No";
        private Typo BodyText => Typo.body2;

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public EventCallback<bool> OpenChanged { get; set; }

        [Parameter]
        public Anchor Anchor { get; set; } = Anchor.End;

        [Parameter]
        public MobEvent Event { get; set; }

        [Parameter]
        public IMobEventManager MobEventManager { get; set; }
    }
}
