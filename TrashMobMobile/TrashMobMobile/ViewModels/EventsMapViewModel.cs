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
            var events = await mobEventManager.GetActiveEventsAsync();

            foreach (var mobEvent in events)
            {
                var pin = new EventPin
                {
                    EventId = mobEvent.Id,
                    Address = $"{mobEvent.StreetAddress}, {mobEvent.City}, {mobEvent.Region}, {mobEvent.Country} {mobEvent.PostalCode}",
                    Label = mobEvent.Name,
                    Name = mobEvent.Name,
                    EventDate = mobEvent.EventDate,
                    Type = PinType.Place,
                    Position = new Position(mobEvent.Latitude, mobEvent.Longitude),
                    // Todo: replace this with a link to a page here
                    Url = $"https://www.trashmob.eco/eventdetails/{mobEvent.Id}",
                };

                // Need to add the pin to both lists
                Map.EventPins.Add(pin);
                Map.Pins.Add(pin);
            }
        }
    }
}