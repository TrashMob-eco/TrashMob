namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

public partial class SearchLitterReportsViewModel : ObservableObject
{
    public SearchLitterReportsViewModel()
    {
    }

    public ObservableCollection<LitterReportViewModel> LitterReportViewModels { get; set; }

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
