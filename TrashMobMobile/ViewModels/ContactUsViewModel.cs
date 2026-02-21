namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Services;

public partial class ContactUsViewModel(IContactRequestManager contactRequestManager, INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly IContactRequestManager contactRequestManager = contactRequestManager;

    [ObservableProperty]
    private string confirmation = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string message = string.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [RelayCommand]
    private async Task SubmitMessage()
    {
        await ExecuteAsync(async () =>
        {
            var contactRequest = new ContactRequest
            {
                Name = Name,
                Email = Email,
                Message = Message,
            };

            await contactRequestManager.AddContactRequestAsync(contactRequest);
            await NotificationService.Notify("Message sent successfully!");
            await Navigation.PopToRootAsync();
        }, "An error has occurred while sending the message. Please wait and try again in a moment.");
    }
}