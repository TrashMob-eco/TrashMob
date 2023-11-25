namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class ContactUsViewModel : ObservableObject
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

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
