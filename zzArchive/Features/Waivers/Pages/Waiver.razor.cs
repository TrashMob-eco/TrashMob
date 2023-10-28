namespace TrashMobMobileApp.Features.Waivers.Pages
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Options;
    using TrashMobMobileApp.Config;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Models;

    public partial class Waiver
    {
        private bool _isLoading;

        public string FullName { get; set; }

        [Inject]
        public IWaiverManager WaiverManager { get; set; }

        [Inject] 
        public IOptions<Settings> Settings { get; set; }

        protected override void OnInitialized()
        {
            TitleContainer.Title = "Sign Waiver";
        }

        private async Task OnSignWaiverAsync()
        {
            _isLoading = true;
            var envelopeRequest = new EnvelopeRequest();
            envelopeRequest.SignerEmail = App.CurrentUser.Email;
            envelopeRequest.CreatedByUserId = App.CurrentUser.Id;
            envelopeRequest.SignerName = FullName;
            envelopeRequest.ReturnUrl = $"{Settings.Value.SiteBaseUrl}/waiversreturn";

            var response = await WaiverManager.GetWaiverEnvelopeAsync(envelopeRequest);

            var uri = new Uri(response.RedirectUrl);
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

            _isLoading = false;
        }
    }
}
