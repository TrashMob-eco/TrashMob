namespace TrashMobMobileApp.Features.Contact.Pages
{
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;

    public partial class ContactUs
    {
#nullable enable
        private MudForm? _contactForm;
#nullable disable
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
