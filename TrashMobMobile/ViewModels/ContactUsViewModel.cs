namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ContactUsViewModel : BaseViewModel
{
    private readonly IContactRequestManager contactRequestManager;

    public ContactUsViewModel(IContactRequestManager contactRequestManager)
    {
        SubmitMessageCommand = new Command(async () => await SubmitMessage());
        this.contactRequestManager = contactRequestManager;
    }

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string email;

    [ObservableProperty]
    string message;

    public ICommand SubmitMessageCommand { get; set; }

    private async Task SubmitMessage()
    {
        var contactRequest = new ContactRequest
        {
            Name = Name,
            Email = Email,
            Message = Message
        };

        await contactRequestManager.AddContactRequestAsync(contactRequest);
    }

    protected override void Validate()
    {
        if (string.IsNullOrEmpty(Name))
        {
            IsValid = false;
            ErrorMessage = "Name cannot be blank.";
            IsErrorMessageVisible = true;
            return;
        }

        if (string.IsNullOrEmpty(Email))
        {
            IsValid = false;
            ErrorMessage = "Email cannot be blank.";
            IsErrorMessageVisible = true;
            return;
        }

        if (!Email.IsValidEmailAddress())
        {
            IsValid = false;
            ErrorMessage = "Email is not a valid address.";
            IsErrorMessageVisible = true;
            return;
        }

        if (string.IsNullOrEmpty(Message))
        {
            IsValid = false;
            ErrorMessage = "Message cannot be blank.";
            IsErrorMessageVisible = true;
            return;
        }

        IsValid = true;
        ErrorMessage = string.Empty;
        IsErrorMessageVisible = false;
    }
}
