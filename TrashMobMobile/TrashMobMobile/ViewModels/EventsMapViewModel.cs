namespace TrashMobMobile.ViewModels
{
    using System.Threading.Tasks;
    using TrashMobMobile.Services;
    using Xamarin.Forms;
    using Xamarin.Forms.Maps;

    public class EventsMapViewModel : BaseViewModel
    {
        private readonly IMobEventManager mobEventManager;
        public Command ReloadEventsCommand { get; }

        public EventsMapViewModel(IMobEventManager mobEventManager)
        {
            Title = "Events Map";
            this.mobEventManager = mobEventManager;
            Map = new Map();
            ReloadEventsCommand = new Command(OnReloadEvents);
            Task.Run(async () => await LoadEvents());
        }

        private async void OnReloadEvents()
        {
            await LoadEvents();
        }

        private async Task LoadEvents()
        {
            var events = await mobEventManager.GetEventsAsync();

            foreach (var mobEvent in events)
            {
                var pin = new Pin
                {
                    Address = mobEvent.City + ", " + mobEvent.Region,
                    Label = mobEvent.Name,
                    Type = PinType.Place,
                    Position = new Position(mobEvent.Latitude, mobEvent.Longitude)
                };

                Map.Pins.Add(pin);
            }
        }

        public Map Map { get; private set; }
    }
}