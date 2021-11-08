namespace TrashMobMobile.Views
{
    using Microsoft.Identity.Client;
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using TrashMobMobile.Features.LogOn;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage(IUserManager userManager)
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
                    Application.Current.MainPage = new AppShell();
                    await Shell.Current.GoToAsync("//main");
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

        private async void OnContactUs(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ContactUsPage());
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

        //private async void OnEditProfile(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var userContext = await B2CAuthenticationService.Instance.EditProfileAsync();
        //        UpdateSignInState(userContext);
        //        UpdateUserInfo(userContext);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Alert if any exception excluding user canceling sign-in dialog
        //        if ((ex as MsalException)?.ErrorCode != "authentication_canceled")
        //        {
        //            await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
        //        }
        //    }
        //}

        //private async void OnResetPassword(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var userContext = await B2CAuthenticationService.Instance.ResetPasswordAsync();
        //        UpdateSignInState(userContext);
        //        UpdateUserInfo(userContext);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Alert if any exception excluding user canceling sign-in dialog
        //        if ((ex as MsalException)?.ErrorCode != "authentication_canceled")
        //        {
        //            await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
        //        }
        //    }
        //}

        private async void OnPasswordReset()
        {
            try
            {
                var userContext = await B2CAuthenticationService.Instance.ResetPasswordAsync();
                UpdateSignInState(userContext);
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
            //btnEditProfile.IsVisible = isSignedIn;
            //btnCallApi.IsVisible = isSignedIn;
            //slUser.IsVisible = isSignedIn;
            //lblApi.Text = "";
        }
    }
}