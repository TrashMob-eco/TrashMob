namespace TrashMobMobile.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using Xamarin.Forms.Maps;

    public abstract class EventBaseViewModel : BaseViewModel
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
        private string eventType;
        private string eventStatus;
        private int eventTypeId;
        private int eventStatusId;
        private int durationHours;
        private int durationMinutes;
        private int maxNumberOfParticipants;
        private double latitude;
        private double longitude;
        private DateTime eventDate;
        private DateTime createdDate;
        private DateTime lastUpdatedDate;
        private bool isEventPublic;
        private bool isUserAttending;
        private bool isUserLoggedIn = false;
        private Position center;
        private List<EventType> eventTypes = new List<EventType>();
        private readonly IEventTypeRestService eventTypeRestService;

        protected IMobEventManager MobEventManager { get; private set; }
        
        protected IUserManager UserManager { get; private set; }

        protected virtual void Map_MapClicked(object sender, MapClickedEventArgs e)
        {

        }

        public EventBaseViewModel(IMobEventManager mobEventManager, IUserManager userManager, IEventTypeRestService eventTypeRestService)
        {
            MobEventManager = mobEventManager;
            UserManager = userManager;
            this.eventTypeRestService = eventTypeRestService;

            Map = new Map();
            Map.MapClicked += Map_MapClicked;

            Task.Run(async () => await LoadEventTypes());
        }

        protected virtual async Task LoadEvent(Guid eventId)
        {
            var mobEvent = await MobEventManager.GetEventAsync(eventId);

            Id = mobEvent.Id;
            Name = mobEvent.Name;
            Description = mobEvent.Description;
            EventTypeId = mobEvent.EventTypeId;
            EventStatusId = mobEvent.EventStatusId;
            CreatedByUserId = mobEvent.CreatedByUserId;
            LastUpdatedByUserId = mobEvent.LastUpdatedByUserId;
            CreatedDate = mobEvent.CreatedDate;
            LastUpdatedDate = mobEvent.LastUpdatedDate;
            EventDate = mobEvent.EventDate;
            StreetAddress = mobEvent.StreetAddress;
            City = mobEvent.City;
            Region = mobEvent.Region;
            Country = mobEvent.Country;
            PostalCode = mobEvent.PostalCode;
            Latitude = mobEvent.Latitude;
            Longitude = mobEvent.Longitude;
            DurationHours = mobEvent.DurationHours;
            DurationMinutes = mobEvent.DurationMinutes;
            MaxNumberOfParticipants = mobEvent.MaxNumberOfParticipants;
            IsEventPublic = mobEvent.IsEventPublic;

            SetEventPin(mobEvent.Name, mobEvent.Region, mobEvent.City, mobEvent.Latitude, mobEvent.Longitude);

            if (App.CurrentUser != null)
            {
                IsUserLoggedIn = true;
                IsUserAttending = await MobEventManager.IsUserAttendingAsync(id, App.CurrentUser.Id);
            }

            EventType = eventTypes.FirstOrDefault(et => et.Id == mobEvent.EventTypeId).Name;
        }

        private async Task LoadEventTypes()
        {
            EventTypes.AddRange(await eventTypeRestService.GetEventTypesAsync());
        }

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

        public string EventType
        {
            get => eventType;
            set => SetProperty(ref eventType, value);
        }

        public string EventStatus
        {
            get => eventStatus;
            set => SetProperty(ref eventStatus, value);
        }

        public int EventTypeId
        {
            get => eventTypeId;
            set => SetProperty(ref eventTypeId, value);
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

        public DateTime EventDate
        {
            get => eventDate;
            set => SetProperty(ref eventDate, value);
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

        public bool IsUserAttending
        {
            get => isUserAttending;
            set => SetProperty(ref isUserAttending, value);
        }

        public bool IsUserLoggedIn
        {
            get => isUserLoggedIn;
            set => SetProperty(ref isUserLoggedIn, value);
        }

        public Map Map { get; private set; }

        public Position Center
        {
            get => center;
            set => SetProperty(ref center, value);
        }

        protected void SetEventPin(string name, string region, string city, double latitude, double longitude)
        {
            var pin = new Pin
            {
                Address = city + ", " + region,
                Label = name,
                Type = PinType.Place,
                Position = new Position(latitude, longitude)
            };

            var mapSpan = new MapSpan(pin.Position, 0.01, 0.01);

            Map.MoveToRegion(mapSpan);

            Map.Pins.Add(pin);
        }
    }
}