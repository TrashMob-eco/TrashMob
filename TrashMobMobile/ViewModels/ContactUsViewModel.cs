namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Services;

public partial class ContactUsViewModel : BaseViewModel
{
    private readonly IContactRequestManager contactRequestManager;

    [ObservableProperty]
    private string confirmation;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string message;

    [ObservableProperty]
    private string name;

    public ContactUsViewModel(IContactRequestManager contactRequestManager)
    {
        this.contactRequestManager = contactRequestManager;
    }

    [RelayCommand]
    private async Task SubmitMessage()
    {
        IsBusy = true;

        var contactRequest = new ContactRequest
        {
            Name = Name,
            Email = Email,
            Message = Message,
        };

        try
        {
            await contactRequestManager.AddContactRequestAsync(contactRequest);

            IsBusy = false;

            await Notify("Message sent successfully!");

            await Navigation.PopToRootAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotifyError("An error has occured while sending the message. Please wait and try again in a moment.");
        }
    }
}