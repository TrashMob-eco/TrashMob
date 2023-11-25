namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class CreateEventViewModel : ObservableObject
{
    public CreateEventViewModel()
    {
    }

    [ObservableProperty]
    EventViewModel eventViewModel;

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
