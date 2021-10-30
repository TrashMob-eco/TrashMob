
namespace TrashMobMobile.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xamarin.Forms;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;

    internal class MobEventsViewModel : BaseViewModel
    {
        public ObservableCollection<MobEvent> MobEvents { get; }
        public Command LoadMobEventsCommand { get; }

        public MobEventsViewModel(IMobEventManager mobEventManager)
        {
            Title = "Browse events";
            MobEvents = new ObservableCollection<MobEvent>();
            this.mobEventManager = mobEventManager;
        }

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                MobEvents.Clear();
                var mobEvents = await mobEventManager.GetEventsAsync();
                foreach (var mobEvent in mobEvents)
                {
                    MobEvents.Add(mobEvent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private Command loadItemsCommand;
        private readonly IMobEventManager mobEventManager;

        public ICommand LoadItemsCommand
        {
            get
            {
                if (loadItemsCommand == null)
                {
                    loadItemsCommand = new Command(LoadItems);
                }

                return loadItemsCommand;
            }
        }

        private void LoadItems()
        {
        }
    }
}
