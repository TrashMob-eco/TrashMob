namespace TrashMobMobile.ViewModels
{
    using Microsoft.Identity.Client;
    using System;
    using System.Threading.Tasks;
    using TrashMobMobile.Features.LogOn;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using TrashMobMobile.Views;
    using Xamarin.Forms;

    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        private readonly IUserManager userManager;

        private string error;

        public LoginViewModel(IUserManager userManager)
        {
            Title = "Login";
            LoginCommand = new Command(OnLogin);
            this.userManager = userManager;
        }

        public string Error
        {
            get => error;
            set => SetProperty(ref error, value);
        }

        private async void OnLogin()
        {
            try
            {
                var userContext = await B2CAuthenticationService.Instance.SignInAsync();

                await VerifyAccount(userContext);
                await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");

                await CheckTermsOfService();
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
                    Error = $"Exception: {ex}";
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

                        // await Navigation.PushModalAsync(userProfilePage);
                    }
                    else
                    {
                        Application.Current.MainPage = new AppShell();
                        await Shell.Current.GoToAsync("//main");
                    }
                };

                // await Navigation.PushModalAsync(termsPage);
            }
        }

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
                await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
                await CheckTermsOfService();
            }
            catch (Exception ex)
            {
                // Alert if any exception excluding user canceling sign-in dialog
                if ((ex as MsalException)?.ErrorCode != "authentication_canceled")
                {
                    Error = $"Exception: {ex}";
                }
            }
        }
    }
}