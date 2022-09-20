using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.Models;

namespace TrashMobMobileApp.Pages.ContactUs.Pages
{
    public partial class ContactUs
    {
        private MudForm? _contactForm;
        private bool _success;
        private bool _isLoading;
        private string[] _errors;
        private ContactRequest _contact = new();
        private bool _hasSubmitted;

        [Inject]
        public IContactRequestManager ContactRequestManager { get; set; }

        protected override void OnInitialized()
        {
            TitleContainer.Title = "Contact Us";
        }

        private async Task OnSubmitAsync()
        {
            if (_success)
            {
                _isLoading = true;
                await ContactRequestManager.AddContactRequestAsync(_contact);
                _isLoading = false;
                _hasSubmitted = true;
            }
            else
            {
                _contactForm?.Validate();
            }
        }
    }
}
