
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

    internal class UserDashboardViewModel : BaseViewModel
    {
        public ObservableRangeCollection<MobEvent> Events { get; }

        public bool ShowFutureEventsOnly { get; set; } = false;

        public AsyncCommand RefreshCommand { get; }

        public AsyncCommand AddCommand { get; }

        public AsyncCommand<MobEvent> SelectedCommand { get; }

        //public AsyncCommand<MobEvent> AttendCommand { get; }

        private readonly IMobEventManager mobEventManager;
        private readonly IUserManager userManager;

        public UserDashboardViewModel(IMobEventManager mobEventManager, IUserManager userManager)
        {
            Title = "My Events";
            Events = new ObservableRangeCollection<MobEvent>();
            this.mobEventManager = mobEventManager;
            this.userManager = userManager;
            RefreshCommand = new AsyncCommand(Refresh);
            AddCommand = new AsyncCommand(Add);
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

            string route;
            if (App.CurrentUser.Id == mobEvent.CreatedByUserId)
            {
                route = $"{nameof(EditEventPage)}?EventId={mobEvent.Id}";
            }
            else
            {
                route = $"{nameof(EventDetailPage)}?EventId={mobEvent.Id}";
            }

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

        private async Task LoadEvents()
        {
            if (App.CurrentUser == null)
            {
                await B2CAuthenticationService.Instance.SignInAsync(userManager);
            }

            IsBusy = true;
            Events.Clear();
            var mobEvents = await mobEventManager.GetUserEventsAsync(App.CurrentUser.Id, ShowFutureEventsOnly);
            Events.AddRange(mobEvents);
            IsBusy = false;
        }
    }
}
