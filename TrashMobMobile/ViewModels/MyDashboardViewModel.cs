namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

public partial class MyDashboardViewModel : BaseViewModel
{
    public MyDashboardViewModel()
    {
    }

    ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = new ObservableCollection<EventViewModel>();
    
    ObservableCollection<EventViewModel> PastEvents { get; set; } = new ObservableCollection<EventViewModel>();

    ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = new ObservableCollection<LitterReportViewModel>();
}
