namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class ViewLitterReportViewModel : ObservableObject
{
    public ViewLitterReportViewModel()
    {
    }

    [ObservableProperty]
    public LitterReportViewModel litterReportViewModel;

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
