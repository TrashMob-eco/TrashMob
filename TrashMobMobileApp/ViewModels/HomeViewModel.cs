namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class HomeViewModel : BaseViewModel
{
    public HomeViewModel()
    {
    }

    [ObservableProperty]
    StatisticsViewModel statisticsViewModel;
}
