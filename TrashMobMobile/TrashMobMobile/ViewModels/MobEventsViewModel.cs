
namespace TrashMobMobile.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;

    internal class MobEventsViewModel : BaseViewModel
    {
        private readonly IMobEventManager mobEventManager;

        public ObservableCollection<MobEvent> MobEvents { get; }
        
        public Command ReloadEventsCommand { get; }

        public MobEventsViewModel(IMobEventManager mobEventManager)
        {
            Title = "Browse events";
            MobEvents = new ObservableCollection<MobEvent>();
            this.mobEventManager = mobEventManager;
            ReloadEventsCommand = new Command(OnReloadEvents);
            Task.Run(async () => await LoadEvents());
        }

        private async void OnReloadEvents()
        {
            await LoadEvents();
        }

        private async Task LoadEvents()
        {
            MobEvents.Clear();
            var mobEvents = await mobEventManager.GetEventsAsync();
            foreach (var mobEvent in mobEvents)
            {
                MobEvents.Add(mobEvent);
            }
        }
    }
}
