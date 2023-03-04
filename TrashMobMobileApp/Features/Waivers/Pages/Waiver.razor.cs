namespace TrashMobMobileApp.Features.Contact.Pages
{
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMob.Models;
    using TrashMobMobileApp.Data;

    public partial class Waiver
    {
#nullable enable
        private MudForm? _waiverForm;
#nullable disable
        private bool _success;
        private bool _isLoading;
        private string[] _errors;
        private WaiverRequest _waiverRequest = new();

        [Inject]
        public IWaiverManager WaiverManager { get; set; }

        protected override void OnInitialized()
        {
            TitleContainer.Title = "Waiver";
        }

        private async Task OnSignWaiverAsync()
        {
            await _waiverForm.Validate();
            if (_success)
            {
                _isLoading = true;
                await WaiverManager.AddContactRequestAsync(_waiverRequest);
                _isLoading = false;
            }
        }
    }
}
