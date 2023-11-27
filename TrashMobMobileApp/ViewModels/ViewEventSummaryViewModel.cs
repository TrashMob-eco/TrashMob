namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class ViewEventSummaryViewModel : BaseViewModel
{
    public ViewEventSummaryViewModel()
    {
    }

    [ObservableProperty]
    EventSummaryViewModel eventSummaryViewModel;
}
