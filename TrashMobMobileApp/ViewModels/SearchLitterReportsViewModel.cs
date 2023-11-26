namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

public partial class SearchLitterReportsViewModel : BaseViewModel
{
    public SearchLitterReportsViewModel()
    {
    }

    public ObservableCollection<LitterReportViewModel> LitterReportViewModels { get; set; } = new ObservableCollection<LitterReportViewModel>();
}
