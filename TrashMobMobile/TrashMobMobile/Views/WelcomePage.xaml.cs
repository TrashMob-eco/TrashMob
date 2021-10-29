namespace TrashMobMobile.Views
{
    using Microsoft.Identity.Client;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading.Tasks;
    using TrashMobMobile.Features.LogOn;
    using TrashMobMobile.Models;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        public UserData CurrentUser { get; set; }

        public WelcomePage()
        {
            InitializeComponent();
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
                    UpdateUserInfo(userContext);
                }
                else
                {
                    var userContext = await B2CAuthenticationService.Instance.SignOutAsync();
                    UpdateSignInState(userContext);
                    UpdateUserInfo(userContext);
                }
            }
            catch (Exception ex)
            {
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

        private async void OnViewEventsMap(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EventsMapPage());
        }
        private async void OnContactUs(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ContactUsPage());
        }

        private HttpRequestMessage GetDefaultHeaders(HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage.Headers.Add("Accept", "application/json, text/plain");
            return httpRequestMessage;
        }

        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private async Task VerifyAccount(UserContext userContext)
        {
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
            httpRequestMessage.Method = HttpMethod.Post;

            httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);
            httpRequestMessage.RequestUri = new Uri(App.ApiEndpoint + "Users");

            var user = new UserData
            {
                Id = Guid.Empty.ToString(),
                NameIdentifier = userContext.NameIdentifier,
                SourceSystemUserName = userContext.SourceSystemUserName ?? "",
                Email = userContext.EmailAddress ?? ""
            };

            httpRequestMessage.Content = JsonContent.Create(user, typeof(UserData), null, jsonSerializerOptions);

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
            string responseString = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(responseString))
            {
                CurrentUser = JsonConvert.DeserializeObject<UserData>(responseString);
            }
        }

        //private async void OnCallApi(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        lblUrl.Text = $"{App.ApiEndpoint}";
        //        lblApi.Text = $"Calling API";
        //        var userContext = await B2CAuthenticationService.Instance.SignInAsync();
        //        var token = userContext.AccessToken;

        //        // Get data from API
        //        HttpClient client = new HttpClient();
        //        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, App.ApiEndpoint);
        //        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //        HttpResponseMessage response = await client.SendAsync(message);
        //        string responseString = await response.Content.ReadAsStringAsync();
        //        if (response.IsSuccessStatusCode)
        //        {
        //            lblApi.Text = $"Response from API {App.ApiEndpoint} | {responseString}";
        //        }
        //        else
        //        {
        //            lblApi.Text = $"Error calling API {App.ApiEndpoint} | {responseString}";
        //        }
        //    }
        //    catch (MsalUiRequiredException ex)
        //    {
        //        await DisplayAlert($"Session has expired, please sign out and back in.", ex.ToString(), "Dismiss");
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
        //    }
        //}

        private async void OnEditProfile(object sender, EventArgs e)
        {
            try
            {
                var userContext = await B2CAuthenticationService.Instance.EditProfileAsync();
                UpdateSignInState(userContext);
                UpdateUserInfo(userContext);
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

        private async void OnResetPassword(object sender, EventArgs e)
        {
            try
            {
                var userContext = await B2CAuthenticationService.Instance.ResetPasswordAsync();
                UpdateSignInState(userContext);
                UpdateUserInfo(userContext);
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

        private async void OnPasswordReset()
        {
            try
            {
                var userContext = await B2CAuthenticationService.Instance.ResetPasswordAsync();
                UpdateSignInState(userContext);
                UpdateUserInfo(userContext);
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

        public void UpdateUserInfo(UserContext userContext)
        {
            lblName.Text = userContext.Name;
            //lblJob.Text = userContext.JobTitle;
            //lblCity.Text = userContext.City;
        }
    }
}