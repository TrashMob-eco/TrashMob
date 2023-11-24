namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class ViewLitterReportViewModel : ObservableObject
{
    public ViewLitterReportViewModel()
    {
    }

    public LitterReportViewModel LitterReportViewModel { get; set; }

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
