namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class ViewEventViewModel : ObservableObject
{
    public ViewEventViewModel()
    {
    }

    [ObservableProperty]
    public EventViewModel eventViewModel;

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
