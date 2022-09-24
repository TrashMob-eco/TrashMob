using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;

namespace TrashMobMobileApp.Pages.Contact.Pages
{
    public partial class ContactUs
    {
        private MudForm? _contactForm;
        private bool _success;
        private bool _isLoading;
        private string[] _errors;
        private ContactRequest _contact = new();

        [Inject]
        public IContactRequestManager ContactRequestManager { get; set; }

        protected override void OnInitialized()
        {
            TitleContainer.Title = "Contact Us";
        }

        private async Task OnSubmitAsync()
        {
            await _contactForm.Validate();
            if (_success)
            {
                _isLoading = true;
                await ContactRequestManager.AddContactRequestAsync(_contact);
                _isLoading = false;
                Snackbar.Add("Submitted!", Severity.Success);
            }
        }
    }
}
