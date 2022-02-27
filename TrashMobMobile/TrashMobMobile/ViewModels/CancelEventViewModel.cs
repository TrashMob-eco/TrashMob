namespace TrashMobMobile.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using TrashMobMobile.Views;
    using Xamarin.Forms;

    [QueryProperty(nameof(EventId), nameof(EventId))]
    public class CancelEventViewModel : EventBaseViewModel
    {
        private string cancelMessage;
        private string cancellationReason;

        public CancelEventViewModel(IMobEventManager mobEventManager, IUserManager userManager, IEventTypeRestService eventTypeRestService) : base(mobEventManager, userManager, eventTypeRestService)
        {
            Title = "Cancel Event";
            SaveCommand = new Command(OnSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        public string EventId
        {
            set
            {
                Task.Run(async () =>
                {
                    await LoadEvent(new Guid(value));
                    CancelMessage = string.Format("Are you sure you want to cancel your TrashMob.eco event '{0}' on {1}?", Name, EventDate.ToString("MMMM dd, yyyy H:mm tt"));
                });
            }
        }

        public string CancellationReason
        {
            get => cancellationReason;
            set => SetProperty(ref cancellationReason, value);
        }

        public Command SaveCommand { get; }

        public Command CancelCommand { get; }

        public string CancelMessage
        {
            get => cancelMessage;
            set => SetProperty(ref cancelMessage, value);
        }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            var cancelEvent = new CancelEvent()
            {
                EventId = Id,
                CancellationReason = cancellationReason
            };

            await MobEventManager.DeleteEventAsync(cancelEvent);

            await Shell.Current.GoToAsync($"//{nameof(MobEventsPage)}");
        }
    }
}