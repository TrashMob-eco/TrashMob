namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class ViewEventViewModel : ObservableObject
{
    public ViewEventViewModel()
    {
    }

    public EventViewModel EventViewModel { get; set; }

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
