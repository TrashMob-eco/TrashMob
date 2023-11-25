namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

public partial class SearchEventsViewModel : ObservableObject
{
    public SearchEventsViewModel()
    {
    }

    public ObservableCollection<EventViewModel> EventViewModels { get; set; } = new ObservableCollection<EventViewModel>();

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
