namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class SubmitLitterReportViewModel : ObservableObject
{
    public SubmitLitterReportViewModel()
    {
    }

    [ObservableProperty]
    public LitterReportViewModel litterReportViewModel;

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
