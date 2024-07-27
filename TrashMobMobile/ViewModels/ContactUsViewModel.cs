namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Services;

public partial class ContactUsViewModel(IContactRequestManager contactRequestManager, INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly IContactRequestManager contactRequestManager = contactRequestManager;

    [ObservableProperty]
    private string confirmation;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string message;

    [ObservableProperty]
    private string name;

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

            await NotificationService.Notify("Message sent successfully!");

            await Navigation.PopToRootAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error has occurred while sending the message. Please wait and try again in a moment.");
        }
    }
}