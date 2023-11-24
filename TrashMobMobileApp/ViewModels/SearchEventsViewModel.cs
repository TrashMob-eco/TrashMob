namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

public partial class SearchEventsViewModel : ObservableObject
{
    public SearchEventsViewModel()
    {
    }

    public ObservableCollection<EventViewModel> EventViewModels { get; set; }

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
