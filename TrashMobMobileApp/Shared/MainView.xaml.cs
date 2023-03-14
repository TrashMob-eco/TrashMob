namespace TrashMobMobileApp;

using Microsoft.AppCenter.Analytics;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System.Text;
using TrashMobMobileApp.Authentication;
using TrashMobMobileApp.Data;
using TrashMobMobileApp.StateContainers;

public partial class MainView : ContentPage
{
    private readonly IUserManager userManager;
    private readonly UserStateInformation userStateInformation;

    public MainView(IUserManager userManager, UserStateInformation userStateInformation)
    {
        InitializeComponent();
        this.userManager = userManager;
        this.userStateInformation = userStateInformation;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (userStateInformation.OnSignOut == null)
        {
            userStateInformation.OnSignOut += async () => await OnSignOutAsync();
        }

        await PerformAuthenticationAsync();
    }

    private async Task PerformAuthenticationAsync()
    {
        try
        {
            IAccount cachedUserAccount = PublicClientSingleton.Instance.MSALClientHelperInstance.FetchSignedInUserFromCache().Result;

            _ = Dispatcher.DispatchAsync(async () =>
            {
                if (cachedUserAccount != null)
                {
                    await Shell.Current.GoToAsync("apphost");
                }
            });

            await PublicClientSingleton.Instance.AcquireTokenSilentAsync();

            var newUserContext = UpdateUserInfo(PublicClientSingleton.Instance.MSALClientHelperInstance.AuthResult);
            await VerifyAccount(userManager, newUserContext);
            UserState.UserContext = newUserContext;
            await Shell.Current.GoToAsync("apphost");
        }
        catch (Exception ex)
        {
            await OnSignOutAsync();
        }
    }

    private async Task OnSignOutAsync()
    {
        await PublicClientSingleton.Instance.SignOutAsync();
        await PerformAuthenticationAsync();
    }

    private static async Task VerifyAccount(IUserManager userManager, UserContext userContext)
    {
        Analytics.TrackEvent("VerifyAccount");
        App.CurrentUser = await userManager.GetUserByEmailAsync(userContext.EmailAddress, userContext);
    }

    private string Base64UrlDecode(string s)
    {
        s = s.Replace('-', '+').Replace('_', '/');
        s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
        var byteArray = Convert.FromBase64String(s);
        var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
        return decoded;
    }

    public UserContext UpdateUserInfo(AuthenticationResult ar)
    {
        var newContext = new UserContext
        {
            IsLoggedOn = false
        };

        JObject user = ParseIdToken(ar.IdToken);

        newContext.AccessToken = ar.AccessToken;
        newContext.GivenName = user["given_name"]?.ToString();
        newContext.EmailAddress = user["email"]?.ToString() ?? user["emailAddress"]?.ToString();

        newContext.IsLoggedOn = true;

        return newContext;
    }

    JObject ParseIdToken(string idToken)
    {
        // Get the piece with actual user info
        idToken = idToken.Split('.')[1];
        idToken = Base64UrlDecode(idToken);
        return JObject.Parse(idToken);
    }
}
