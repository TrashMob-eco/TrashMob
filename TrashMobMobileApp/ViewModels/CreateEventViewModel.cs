namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class CreateEventViewModel : ObservableObject
{
    public CreateEventViewModel()
    {
    }

    public EventViewModel EventViewModel { get; set; }

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
