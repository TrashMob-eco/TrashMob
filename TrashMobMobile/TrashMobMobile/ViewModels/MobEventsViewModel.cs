
namespace TrashMobMobile.ViewModels
{
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using TrashMobMobile.Views;
    using MvvmHelpers;
    using MvvmHelpers.Commands;
    using TrashMobMobile.Features.LogOn;

    internal class MobEventsViewModel : BaseViewModel
    {
        public ObservableRangeCollection<MobEvent> Events { get; }

        public AsyncCommand RefreshCommand { get; }

        public AsyncCommand AddCommand { get; }

        public AsyncCommand<MobEvent> SelectedCommand { get; }

        //public AsyncCommand<MobEvent> AttendCommand { get; }

        private readonly IMobEventManager mobEventManager;
        private readonly IUserManager userManager;

        public MobEventsViewModel(IMobEventManager mobEventManager, IUserManager userManager)
        {
            Title = "Upcoming Events";
            Events = new ObservableRangeCollection<MobEvent>();
            this.mobEventManager = mobEventManager;
            this.userManager = userManager;
            RefreshCommand = new AsyncCommand(Refresh);
            AddCommand = new AsyncCommand(Add);
            //AttendCommand = new AsyncCommand<MobEvent>(Attend);
            SelectedCommand = new AsyncCommand<MobEvent>(Selected);
        }

        private async Task Refresh()
        {
            await LoadEvents();
        }

        private async Task Selected(MobEvent mobEvent)
        {
            if (mobEvent == null)
            {
                return;
            }

            var route = $"{nameof(EventDetailPage)}?EventId={mobEvent.Id}";
            await Shell.Current.GoToAsync(route);
        }

        private async Task Add()
        {
            if (App.CurrentUser == null)
            {
                await B2CAuthenticationService.Instance.SignInAsync(userManager);
            }

            var route = $"{nameof(AddEventPage)}";
            await Shell.Current.GoToAsync(route);
        }

        //private async Task Attend(MobEvent mobEvent)
        //{
        //    // Todo
        //}

        private async Task LoadEvents()
        {
            if (App.CurrentUser == null)
            {
                await B2CAuthenticationService.Instance.SignInAsync(userManager);
            }

            IsBusy = true;
            Events.Clear();
            var mobEvents = await mobEventManager.GetEventsAsync();
            Events.AddRange(mobEvents);
            IsBusy = false;
        }
    }
}
