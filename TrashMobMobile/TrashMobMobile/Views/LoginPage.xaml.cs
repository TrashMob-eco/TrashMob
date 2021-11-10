namespace TrashMobMobile.Views
{
    using Microsoft.Identity.Client;
    using System;
    using System.Threading.Tasks;
    using TrashMobMobile.Features.LogOn;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage(IUserManager userManager)
        {
            InitializeComponent();
            this.userManager = userManager;
        }

        private async void OnSignInSignOut(object sender, EventArgs e)
        {
            try
            {
                if (btnSignInSignOut.Text == "Sign in")
                {
                    var userContext = await B2CAuthenticationService.Instance.SignInAsync();

                    await VerifyAccount(userContext);
                    UpdateSignInState(userContext);
                    await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");

                    await CheckTermsOfService();
                }
                else
                {
                    var userContext = await B2CAuthenticationService.Instance.SignOutAsync();
                    UpdateSignInState(userContext);
                    await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
                }
            }
            catch (Exception ex)
            {
                await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
                // Checking the exception message 
                // should ONLY be done for B2C
                // reset and not any other error.
                if (ex.Message.Contains("AADB2C90118"))
                {
                    OnPasswordReset();
                }
                // Alert if any exception excluding user canceling sign-in dialog
                else if ((ex as MsalException)?.ErrorCode != "authentication_canceled")
                {
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
                }
            }
        }

        private async Task CheckTermsOfService()
        {
            var isPrivacyPolicyOutOfDate = App.CurrentUser.DateAgreedToPrivacyPolicy == null || App.CurrentUser.DateAgreedToPrivacyPolicy.Value < Constants.PrivacyPolicyDate;
            var isTermsOfServiceOutOfDate = App.CurrentUser.DateAgreedToTermsOfService == null || App.CurrentUser.DateAgreedToTermsOfService.Value < Constants.TermsOfServiceDate;

            if (isPrivacyPolicyOutOfDate || isTermsOfServiceOutOfDate || string.IsNullOrWhiteSpace(App.CurrentUser.TermsOfServiceVersion) || string.IsNullOrWhiteSpace(App.CurrentUser.PrivacyPolicyVersion))
            {
                // Redirect to Terms of Service Page
                var termsPage = new TermsAndConditionsPage();

                termsPage.Disappearing += async (sender2, e2) =>
                {
                    // If the user name is not present, redirect to User Profile Page to allow user to fill it in
                    if (string.IsNullOrWhiteSpace(App.CurrentUser.UserName) || App.CurrentUser.UserName.Contains("joe"))
                    {
                        var userProfilePage = new UserProfilePage();
                        userProfilePage.Disappearing += async (sender3, e3) =>
                        {
                            Application.Current.MainPage = new AppShell();
                            await Shell.Current.GoToAsync("//main");
                        };

                        await Navigation.PushModalAsync(userProfilePage);
                    }
                    else
                    {
                        Application.Current.MainPage = new AppShell();
                        await Shell.Current.GoToAsync("//main");
                    }
                };

                await Navigation.PushModalAsync(termsPage);
            }
        }

        private readonly IUserManager userManager;

        private async Task VerifyAccount(UserContext userContext)
        {
            var user = new User
            {
                Id = Guid.Empty.ToString(),
                NameIdentifier = userContext.NameIdentifier,
                SourceSystemUserName = userContext.SourceSystemUserName ?? "",
                Email = userContext.EmailAddress ?? ""
            };

            App.CurrentUser = await userManager.AddUserAsync(user);
        }

        private async void OnPasswordReset()
        {
            try
            {
                var userContext = await B2CAuthenticationService.Instance.ResetPasswordAsync();

                await VerifyAccount(userContext);
                UpdateSignInState(userContext);
                await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
                await CheckTermsOfService();
            }
            catch (Exception ex)
            {
                // Alert if any exception excluding user canceling sign-in dialog
                if ((ex as MsalException)?.ErrorCode != "authentication_canceled")
                {
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
                }
            }
        }

        private void UpdateSignInState(UserContext userContext)
        {
            var isSignedIn = userContext.IsLoggedOn;
            btnSignInSignOut.Text = isSignedIn ? "Sign out" : "Sign in";
        }
    }
}