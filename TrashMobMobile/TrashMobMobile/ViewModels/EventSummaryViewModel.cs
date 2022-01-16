namespace TrashMobMobile.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using Xamarin.Forms;

    [QueryProperty(nameof(EventId), nameof(EventId))]
    public class EventSummaryViewModel : BaseViewModel
    {
        private Guid eventId;
        private int actualNumberOfAttendees;
        private int numberOfBags;
        private int numberOfBuckets;
        private int durationInMinutes;
        private string notes;
        private Guid CreatedByUserId = Guid.Empty;
        private DateTimeOffset? CreatedDate = DateTime.Now;
        private Guid LastUpdatedByUserId = Guid.Empty;
        private DateTimeOffset? LastUpdatedDate = DateTime.Now;
        private readonly IMobEventManager mobEventManager;

        public EventSummaryViewModel(IMobEventManager mobEventManager, IUserManager userManager)
        {
            Title = "Event Summary";
            CancelCommand = new Command(OnCancel);
            SaveCommand = new Command(OnSave, ValidateSave);
            this.mobEventManager = mobEventManager;
        }

        public string EventId
        {
            set
            {
                Task.Run(async () => await LoadEventSummary(new Guid(value)));
            }
        }

        public Command SaveCommand { get; }

        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        private bool ValidateSave()
        {
            return true;
        }

        public Guid Id
        {
            get => eventId;
            set => SetProperty(ref eventId, value);
        }

        public int ActualNumberOfAttendees
        {
            get => actualNumberOfAttendees;
            set => SetProperty(ref actualNumberOfAttendees, value);
        }

        public int NumberOfBags
        {
            get => numberOfBags;
            set => SetProperty(ref numberOfBags, value);
        }

        public int NumberOfBuckets
        {
            get => numberOfBuckets;
            set => SetProperty(ref numberOfBuckets, value);
        }

        public int DurationInMinutes
        {
            get => durationInMinutes;
            set => SetProperty(ref durationInMinutes, value);
        }

        public string Notes
        {
            get => notes;
            set => SetProperty(ref notes, value);
        }

        protected virtual async Task LoadEventSummary(Guid eventId)
        {
            Id = eventId;

            var eventSummary = await mobEventManager.GetEventSummaryAsync(eventId);

            if (eventSummary != null)
            {
                NumberOfBags = eventSummary.NumberOfBags;
                NumberOfBuckets = eventSummary.NumberOfBuckets;
                ActualNumberOfAttendees = eventSummary.ActualNumberOfAttendees;
                DurationInMinutes = eventSummary.DurationInMinutes;
                Notes = eventSummary.Notes;
                CreatedByUserId = eventSummary.CreatedByUserId;
                CreatedDate = eventSummary.CreatedDate;
                LastUpdatedByUserId = eventSummary.LastUpdatedByUserId;
                LastUpdatedDate = eventSummary.LastUpdatedDate;
            }
        }

        protected async void OnSave()
        {
            var eventSummary = new EventSummary
            {
                EventId = Id,
                NumberOfBags = NumberOfBags,
                NumberOfBuckets = NumberOfBuckets,
                ActualNumberOfAttendees = ActualNumberOfAttendees,
                DurationInMinutes = DurationInMinutes,
                Notes = Notes,
                LastUpdatedByUserId = App.CurrentUser.Id,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                CreatedDate = CreatedDate,
            };

            if (CreatedByUserId == Guid.Empty)
            {
                eventSummary.CreatedByUserId = App.CurrentUser.Id;
                CreatedDate = DateTime.UtcNow;

                eventSummary = await mobEventManager.AddEventSummaryAsync(eventSummary);
            }
            else
            {
                eventSummary = await mobEventManager.UpdateEventSummaryAsync(eventSummary);
            }

            Id = eventSummary.EventId;
            NumberOfBags = eventSummary.NumberOfBags;
            NumberOfBuckets = eventSummary.NumberOfBuckets;
            ActualNumberOfAttendees = eventSummary.ActualNumberOfAttendees;
            DurationInMinutes = eventSummary.DurationInMinutes;
            Notes = eventSummary.Notes;
            CreatedByUserId = eventSummary.CreatedByUserId;
            CreatedDate = eventSummary.CreatedDate;
            LastUpdatedByUserId = eventSummary.LastUpdatedByUserId;
            LastUpdatedDate = eventSummary.LastUpdatedDate;

            await Shell.Current.GoToAsync("..");
        }
    }
}