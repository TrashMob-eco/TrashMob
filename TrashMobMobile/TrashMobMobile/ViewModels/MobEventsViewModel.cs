
namespace TrashMobMobile.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using TrashMobMobile.Views;
    using MvvmHelpers;
    using MvvmHelpers.Commands;

    internal class MobEventsViewModel : BaseViewModel
    {
        public ObservableRangeCollection<MobEvent> Events { get; }

        public AsyncCommand RefreshCommand { get; }

        public AsyncCommand AddCommand { get; }

        public AsyncCommand<MobEvent> SelectedCommand { get; }

        public AsyncCommand<MobEvent> AttendCommand { get; }

        private readonly IMobEventManager mobEventManager;

        public MobEventsViewModel(IMobEventManager mobEventManager)
        {
            Title = "Upcoming Events";
            Events = new ObservableRangeCollection<MobEvent>();
            this.mobEventManager = mobEventManager;
            RefreshCommand = new AsyncCommand(Refresh);
            AddCommand = new AsyncCommand(Add);
            AttendCommand = new AsyncCommand<MobEvent>(Attend);
            SelectedCommand = new AsyncCommand<MobEvent>(Selected);
        }

        private async Task Refresh()
        {
            await LoadEvents();
        }

        private async Task Selected(MobEvent mobEvent)
        {
            var route = $"{nameof(EventDetailPage)}?id={mobEvent.Id}";
            await Shell.Current.GoToAsync(route);
        }

        private async Task Add()
        {
            await Shell.Current.GoToAsync($"{nameof(ManageEventPage)}");
        }

        private async Task Attend(MobEvent mobEvent)
        {
            // Todo
        }

        private async Task LoadEvents()
        {
            IsBusy = true;
            Events.Clear();
            var mobEvents = await mobEventManager.GetEventsAsync();
            Events.AddRange(mobEvents);
            IsBusy = false;
        }
    }
}
