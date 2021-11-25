namespace TrashMobMobile.ViewModels
{
    using System.Threading.Tasks;
    using TrashMobMobile.Controls;
    using TrashMobMobile.Services;
    using Xamarin.Forms.Maps;

    public class EventsMapViewModel : BaseViewModel
    {
        private readonly IMobEventManager mobEventManager;

        public EventMap Map { get; private set; }

        public EventsMapViewModel(IMobEventManager mobEventManager)
        {
            Title = "Events Map";
            this.mobEventManager = mobEventManager;
            Map = new EventMap()
            {
                MapType = MapType.Street
            };
            
            Task.Run(async () => await LoadEvents());
        }

        private async Task LoadEvents()
        {
            var events = await mobEventManager.GetEventsAsync();

            foreach (var mobEvent in events)
            {
                var pin = new EventPin
                {
                    EventId = mobEvent.Id,
                    Address = mobEvent.City + ", " + mobEvent.Region,
                    Label = mobEvent.Name,
                    Type = PinType.Place,
                    Position = new Position(mobEvent.Latitude, mobEvent.Longitude)
                };

                Map.EventPins.Add(pin);
            }
        }
    }
}