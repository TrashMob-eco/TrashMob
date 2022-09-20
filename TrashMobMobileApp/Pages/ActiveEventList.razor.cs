using Microsoft.AspNetCore.Components;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;

namespace TrashMobMobileApp.Pages
{
    public partial class ActiveEventList
    {
        private List<MobEvent> _mobEvents = new();
        private bool _isLoading;

        [Inject]
        public IMobEventManager MobEventManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            _mobEvents = (await MobEventManager.GetActiveEventsAsync()).ToList();
            var randomEvents = new List<MobEvent>
            {
                new MobEvent
                {
                    Name = "Event Random 1",
                    Description = "Event Description 1"
                },
                new MobEvent
                {
                    Name = "Event Random 2",
                    Description = "Event Description 2"
                },
                new MobEvent
                {
                    Name = "Event Random 3",
                    Description = "Event Description 3"
                }
            };
            _mobEvents.AddRange(randomEvents);
            TitleContainer.Title = "Active Events";
            _isLoading = false;
        }
    }
}
