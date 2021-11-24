namespace TrashMobMobile.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using Xamarin.Forms;
    using Xamarin.Forms.Maps;

    [QueryProperty(nameof(EventId), nameof(EventId))]
    public class EditEventViewModel : EventBaseViewModel
    {
        private DateTime eDate;
        private TimeSpan eTime;

        private readonly IMapRestService mapRestService;

        public EditEventViewModel(IMobEventManager mobEventManager, IUserManager userManager, IEventTypeRestService eventTypeRestService, IMapRestService mapRestService) : base(mobEventManager, userManager, eventTypeRestService)
        {
            Title = "Edit Event";
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            this.mapRestService = mapRestService;
        }

        public string EventId
        {
            set
            {
                Task.Run(async () => await LoadEvent(new Guid(value)));
            }
        }

        public Command SaveCommand { get; }

        public Command CancelCommand { get; }

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

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(Name) 
                && SelectedEventType != null
                && EDate != null
                && ETime != null
                && !string.IsNullOrWhiteSpace(Description) 
                && Map.Pins.Count == 1;
        }

        protected override async void Map_MapClicked(object sender, MapClickedEventArgs e)
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

        private async void OnSave()
        {
            MobEvent mobEvent = new MobEvent()
            {
                Id = Id,
                Name = Name,
                Description = Description,
                EventTypeId = SelectedEventType.Id,
                EventStatusId = EventStatusId,
                CreatedByUserId = App.CurrentUser.Id,
                LastUpdatedByUserId = App.CurrentUser.Id,
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

            await MobEventManager.UpdateEventAsync(mobEvent);

            await Shell.Current.GoToAsync("..");
        }
    }
}