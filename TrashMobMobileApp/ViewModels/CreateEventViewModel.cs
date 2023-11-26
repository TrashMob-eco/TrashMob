namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class CreateEventViewModel : BaseViewModel
{
    public CreateEventViewModel()
    {
    }

    [ObservableProperty]
    EventViewModel eventViewModel;
}
