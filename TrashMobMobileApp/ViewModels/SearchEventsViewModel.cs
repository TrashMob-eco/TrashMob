namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

public partial class SearchEventsViewModel : BaseViewModel
{
    public SearchEventsViewModel()
    {
    }

    public ObservableCollection<EventViewModel> EventViewModels { get; set; } = new ObservableCollection<EventViewModel>();
}
