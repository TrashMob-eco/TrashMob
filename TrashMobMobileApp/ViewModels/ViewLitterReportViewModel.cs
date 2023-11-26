namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class ViewLitterReportViewModel : BaseViewModel
{
    public ViewLitterReportViewModel()
    {
    }

    [ObservableProperty]
    public LitterReportViewModel litterReportViewModel;
}
