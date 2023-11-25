namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class EditEventViewModel : ObservableObject
{
    public EditEventViewModel()
    {
    }

    [ObservableProperty]
    EventViewModel eventViewModel;

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
