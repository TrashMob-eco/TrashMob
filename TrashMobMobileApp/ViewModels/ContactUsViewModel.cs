namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class ContactUsViewModel : ObservableObject
{
    private string name;

    private string email;

    private string message;

    public ContactUsViewModel()
    {
    }

    public string Name
    {
        get => name;

        set
        {
            name = value;
            OnPropertyChanged();
        }
    }

    public string Email
    {
        get => email;

        set
        {
            email = value;
            OnPropertyChanged();
        }
    }

    public string Message
    {
        get => message;

        set
        {
            message = value;
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
