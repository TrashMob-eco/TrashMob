namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class ViewEventViewModel : BaseViewModel
{
    public ViewEventViewModel()
    {
    }

    [ObservableProperty]
    public EventViewModel eventViewModel;
}
