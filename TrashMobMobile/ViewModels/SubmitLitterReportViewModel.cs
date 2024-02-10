namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class SubmitLitterReportViewModel : BaseViewModel
{
    public SubmitLitterReportViewModel()
    {
    }

    [ObservableProperty]
    public LitterReportViewModel litterReportViewModel;
}
