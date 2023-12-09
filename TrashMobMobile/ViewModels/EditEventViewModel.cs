namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class EditEventViewModel :  BaseViewModel
{
    public EditEventViewModel()
    {
    }

    [ObservableProperty]
    EventViewModel eventViewModel;
}
