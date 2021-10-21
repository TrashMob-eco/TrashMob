using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using TrashMobMobile.Models;
using TrashMobMobile.Services;

namespace TrashMobMobile.ViewModels
{
    class MobEventsViewModel : BaseViewModel
    {
        public ObservableCollection<MobEvent> MobEvents { get; }
        public Command LoadMobEventsCommand { get; }

        public MobEventsViewModel()
        {
            Title = "Browse events";
            MobEvents = new ObservableCollection<MobEvent>();
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                MobEvents.Clear();
                var mobEvents = await MobEventManager.GetEventsAsync();
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
    }
}
