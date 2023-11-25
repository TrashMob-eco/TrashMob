namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

public partial class MyDashboardViewModel : ObservableObject
{
    public MyDashboardViewModel()
    {
    }

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;

    ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = new ObservableCollection<EventViewModel>();
    
    ObservableCollection<EventViewModel> PastEvents { get; set; } = new ObservableCollection<EventViewModel>();

    ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = new ObservableCollection<LitterReportViewModel>();
}
