namespace TrashMobMobileApp.Features.Waivers.Pages
{
    using Microsoft.AspNetCore.Components;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Models;

    public partial class Waiver
    {
        private bool _isLoading;

        public string FullName { get; set; }

        [Inject]
        public IWaiverManager WaiverManager { get; set; }

        protected override void OnInitialized()
        {
            TitleContainer.Title = "Waiver";
        }

        private async Task OnSignWaiverAsync()
        {
            _isLoading = true;
            var envelopeRequest = new EnvelopeRequest();
            envelopeRequest.SignerEmail = App.CurrentUser.Email;
            envelopeRequest.CreatedByUserId = App.CurrentUser.Id;
            envelopeRequest.SignerName = FullName;

            var response = await WaiverManager.GetWaiverEnvelopeAsync(envelopeRequest);
            _isLoading = false;
        }
    }
}
