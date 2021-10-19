using System.Collections.ObjectModel;
using TrashMobMobile.Models;

namespace TrashMobMobile.ViewModels
{
    class MobEventsViewModel : BaseViewModel
    {
        public ObservableCollection<MobEvent> MobEvents { get; }

        public MobEventsViewModel()
        {
            Title = "Browse events";
            MobEvents = new ObservableCollection<MobEvent>();
        }
    }
}
