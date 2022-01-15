namespace TrashMobMobile.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using TrashMobMobile.Features.LogOn;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using TrashMobMobile.Views;
    using Xamarin.Forms;

    [QueryProperty(nameof(EventId), nameof(EventId))]
    public class EventDetailViewModel : EventBaseViewModel
    {
        public EventDetailViewModel(IMobEventManager mobEventManager, IUserManager userManager, IEventTypeRestService eventTypeRestService) : base(mobEventManager, userManager, eventTypeRestService)
        {
            Title = "Event Details";
            AttendCommand = new Command(OnAttend);
            UnattendCommand = new Command(OnUnattend);
            CancelCommand = new Command(OnCancel);
            EventSummaryDetailCommand = new Command(OnEventSummaryDetail);
            PropertyChanged +=
                (_, __) => AttendCommand.ChangeCanExecute();
        }

        public string EventId
        {
            set
            {
                Task.Run(async () => await LoadEvent(new Guid(value)));
            }
        }

        public Command AttendCommand { get; }

        public Command UnattendCommand { get; }

        public Command EventSummaryDetailCommand { get; }

        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnAttend()
        {
            if (App.CurrentUser == null)
            {
                await B2CAuthenticationService.Instance.SignInAsync(UserManager);
                IsUserLoggedIn = true;
            }

            var eventAttendee = new EventAttendee
            {
                EventId = Id,
                UserId = App.CurrentUser.Id
            };

            await MobEventManager.AddEventAttendeeAsync(eventAttendee);

            await Shell.Current.GoToAsync("..");
        }

        private async void OnUnattend()
        {
            if (App.CurrentUser == null)
            {
                await B2CAuthenticationService.Instance.SignInAsync(UserManager);
                IsUserLoggedIn = true;
            }

            var eventAttendee = new EventAttendee
            {
                EventId = Id,
                UserId = App.CurrentUser.Id
            };

            await MobEventManager.RemoveEventAttendeeAsync(eventAttendee);

            await Shell.Current.GoToAsync("..");
        }

        private async void OnEventSummaryDetail()
        {
            await Shell.Current.GoToAsync($"{nameof(EventSummaryDetailPage)}?EventId={Id}");
        }
    }
}