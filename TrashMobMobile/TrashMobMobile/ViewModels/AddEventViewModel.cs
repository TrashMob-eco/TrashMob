namespace TrashMobMobile.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using Xamarin.Forms;
    using Xamarin.Forms.Maps;

    public class AddEventViewModel : BaseViewModel
    {
        private Guid id;
        private Guid createdByUserId;
        private Guid lastUpdatedByUserId;
        private string name;
        private string description;
        private string streetAddress;
        private string city;
        private string region;
        private string country;
        private string postalCode;
        private EventType selectedEventType;
        private string eventStatus;
        private int eventStatusId;
        private int durationHours;
        private int durationMinutes;
        private int maxNumberOfParticipants;
        private double latitude;
        private double longitude;
        private DateTime eDate;
        private TimeSpan eTime;
        private DateTime createdDate;
        private DateTime lastUpdatedDate;
        private bool isEventPublic;
        private List<EventType> eventTypes = new List<EventType>();

        private readonly IMobEventManager mobEventManager;
        private readonly IEventTypeRestService eventTypeRestService;
        private readonly IMapRestService mapRestService;
        private Position center;

        public AddEventViewModel(IMobEventManager mobEventManager, IEventTypeRestService eventTypeRestService, IMapRestService mapRestService)
        {
            Title = "Add Event";
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            this.mobEventManager = mobEventManager;
            this.eventTypeRestService = eventTypeRestService;
            this.mapRestService = mapRestService;
            Map = new Map();
            Map.MapClicked += Map_MapClicked;
            Id = Guid.Empty;
            Task.Run(async () => await LoadEventTypes());

            // Set default start time
            EDate = DateTime.Now;
            ETime = TimeSpan.FromHours(9);            
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public List<EventType> EventTypes
        {
            get => eventTypes;
        }

        public Guid Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public Guid CreatedByUserId
        {
            get => createdByUserId;
            set => SetProperty(ref createdByUserId, value);
        }

        public Guid LastUpdatedByUserId
        {
            get => lastUpdatedByUserId;
            set => SetProperty(ref lastUpdatedByUserId, value);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public string StreetAddress
        {
            get => streetAddress;
            set => SetProperty(ref streetAddress, value);
        }

        public string City
        {
            get => city;
            set => SetProperty(ref city, value);
        }

        public string Region
        {
            get => region;
            set => SetProperty(ref region, value);
        }

        public string Country
        {
            get => country;
            set => SetProperty(ref country, value);
        }

        public string PostalCode
        {
            get => postalCode;
            set => SetProperty(ref postalCode, value);
        }

        public string EventStatus
        {
            get => eventStatus;
            set => SetProperty(ref eventStatus, value);
        }

        public EventType SelectedEventType
        {
            get => selectedEventType;
            set => SetProperty(ref selectedEventType, value);
        }

        public int EventStatusId
        {
            get => eventStatusId;
            set => SetProperty(ref eventStatusId, value);
        }

        public int DurationHours
        {
            get => durationHours;
            set => SetProperty(ref durationHours, value);
        }

        public int DurationMinutes
        {
            get => durationMinutes;
            set => SetProperty(ref durationMinutes, value);
        }

        public int MaxNumberOfParticipants
        {
            get => maxNumberOfParticipants;
            set => SetProperty(ref maxNumberOfParticipants, value);
        }

        public double Latitude
        {
            get => latitude;
            set => SetProperty(ref latitude, value);
        }

        public double Longitude
        {
            get => longitude;
            set => SetProperty(ref longitude, value);
        }

        public DateTime EDate
        {
            get => eDate;
            set => SetProperty(ref eDate, value);
        }

        public TimeSpan ETime
        {
            get => eTime;
            set => SetProperty(ref eTime, value);
        }

        public DateTime CreatedDate
        {
            get => createdDate;
            set => SetProperty(ref createdDate, value);
        }

        public DateTime LastUpdatedDate
        {
            get => lastUpdatedDate;
            set => SetProperty(ref lastUpdatedDate, value);
        }

        public bool IsEventPublic
        {
            get => isEventPublic;
            set => SetProperty(ref isEventPublic, value);
        }

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(name) 
                && SelectedEventType != null
                && EDate != null
                && ETime != null
                && !string.IsNullOrWhiteSpace(Description) 
                && Map.Pins.Count == 1;
        }

        public Map Map { get; private set; }

        public Position Center
        {
            get => center;
            set => SetProperty(ref center, value);
        }

        private void SetEventPin(string name, string region, string city, double latitude, double longitude)
        {
            var pin = new Pin
            {
                Address = city + ", " + region,
                Label = name ?? "New Event",
                Type = PinType.Place,
                Position = new Position(latitude, longitude)
            };

            var mapSpan = new MapSpan(pin.Position, 0.01, 0.01);

            Map.MoveToRegion(mapSpan);

            Map.Pins.Add(pin);
        }

        private async void Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            if (e != null)
            {
                var position = e.Position;
                Latitude = position.Latitude;
                Longitude = position.Longitude;

                var address = await mapRestService.GetAddressAsync(position.Latitude, position.Longitude);
                StreetAddress = address.StreetAddress;
                City = address.City;
                Country = address.Country;
                Region = address.Region;
                PostalCode = address.PostalCode;

                if (Map.Pins.Count > 0)
                {
                    Map.Pins[0].Position = new Position(position.Latitude, position.Longitude);
                }
                else
                {
                    SetEventPin(Name, address.Region, address.City, position.Latitude, position.Longitude);
                }
            }
        }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async Task LoadEventTypes()
        {
            EventTypes.AddRange(await eventTypeRestService.GetEventTypesAsync());
        }

        private async void OnSave()
        {
            MobEvent mobEvent = new MobEvent()
            {
                Id = Id,
                Name = Name,
                Description = Description,
                EventTypeId = SelectedEventType.Id,
                EventStatusId = EventStatusId,
                CreatedByUserId = new Guid(App.CurrentUser.Id),
                LastUpdatedByUserId = new Guid(App.CurrentUser.Id),
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                EventDate = EDate + ETime,
                StreetAddress = StreetAddress ?? string.Empty,
                City = City ?? string.Empty,
                Region = Region ?? string.Empty,
                Country = Country ?? string.Empty,
                PostalCode = PostalCode ?? string.Empty,
                Latitude = Latitude,
                Longitude = Longitude,
                DurationHours = DurationHours,
                DurationMinutes = DurationMinutes,
                MaxNumberOfParticipants = MaxNumberOfParticipants,
                IsEventPublic = IsEventPublic,
            };

            await mobEventManager.AddEventAsync(mobEvent);

            await Shell.Current.GoToAsync("..");
        }
    }
}