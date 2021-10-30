namespace TrashMobMobile.ViewModels
{
    using System;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using Xamarin.Forms;

    public class ContactUsViewModel : BaseViewModel
    {
        private string name;
        private string email;
        private string message;
        private readonly IContactRequestManager contactRequestManager;

        public ContactUsViewModel(IContactRequestManager contactRequestManager)
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            this.contactRequestManager = contactRequestManager;
        }

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(email)
                && !string.IsNullOrWhiteSpace(name) 
                && !string.IsNullOrWhiteSpace(message);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public Command SaveCommand { get; }

        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            ContactRequest contactRequest = new ContactRequest()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Name,
                Email = Email,
                Message = Message
            };

            await contactRequestManager.AddContactRequestAsync(contactRequest);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
