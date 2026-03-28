namespace TrashMobMobile.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using TrashMobMobile.Services;

    /// <summary>
    /// ViewModel for the PRIVO identity verification page.
    /// </summary>
    public partial class VerifyIdentityViewModel(
        IPrivoConsentRestService privoConsentRestService,
        IUserManager userManager,
        INotificationService notificationService)
        : BaseViewModel(notificationService)
    {
        [ObservableProperty]
        private bool isVerified;

        [ObservableProperty]
        private bool isPending;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        public async Task Init()
        {
            await ExecuteAsync(async () =>
            {
                IsVerified = userManager.CurrentUser.IsIdentityVerified;

                if (IsVerified)
                {
                    StatusMessage = "Your identity has been verified. You can add children aged 13-17 as dependents.";
                    return;
                }

                var status = await privoConsentRestService.GetVerificationStatusAsync();

                // Status 1 = Pending
                if (status != null && status.Status == 1)
                {
                    IsPending = true;
                    StatusMessage = "Verification in progress. Please complete the verification in your browser.";
                }
                else
                {
                    StatusMessage = "Verify your identity to add children aged 13-17 as dependents.";
                }
            }, "Failed to load verification status.");
        }

        [RelayCommand]
        private async Task StartVerification()
        {
            await ExecuteAsync(async () =>
            {
                var consent = await privoConsentRestService.InitiateAdultVerificationAsync();

                if (!string.IsNullOrEmpty(consent.ConsentUrl))
                {
                    // Open PRIVO verification in system browser
                    await Browser.Default.OpenAsync(consent.ConsentUrl, BrowserLaunchMode.SystemPreferred);
                    IsPending = true;
                    StatusMessage = "Complete the verification in your browser. Return here when done.";
                }
                else
                {
                    await NotificationService.Notify("Verification started. Check your email for next steps.");
                }
            }, "Failed to start verification. Please try again.");
        }

        [RelayCommand]
        private async Task RefreshStatus()
        {
            await ExecuteAsync(async () =>
            {
                var status = await privoConsentRestService.GetVerificationStatusAsync();

                // Status 2 = Verified
                if (status != null && status.Status == 2)
                {
                    IsVerified = true;
                    IsPending = false;
                    StatusMessage = "Your identity has been verified!";
                    await NotificationService.Notify("Identity verified successfully!");
                }
                else if (status != null && status.Status == 1)
                {
                    StatusMessage = "Verification still in progress. Please try again in a moment.";
                }
            }, "Failed to check verification status.");
        }
    }
}
