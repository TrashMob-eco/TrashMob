namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class ContactUsViewModel : BaseViewModel
{
    public ContactUsViewModel()
    {
    }

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string email;

    [ObservableProperty]
    string message;
}
