namespace TrashMobMobile.Authentication;

using System.Diagnostics;
using System.Text;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using TrashMobMobile.Data;

public class AuthService : IAuthService
{
    private readonly IUserManager userManager;

    private string accessToken = string.Empty;

    private DateTimeOffset expiresOn;
    private IPublicClientApplication pca;

    private string userEmail = string.Empty;

    public AuthService(IUserManager userManager)
    {
        this.userManager = userManager;
        InitializeClient();
    }

    public async Task<SignInResult> SignInAsync()
    {
        if (pca == null)
        {
            InitializeClient();
        }

        _ = await pca.GetAccountsAsync();

        try
        {
            return await SignInSilentAsync();
        }
        catch (Exception ex)
        {
            // TODO: handle
            Debug.WriteLine($"MSAL Silent Error: {ex.Message}");
            return new SignInResult
            {
                Succeeded = false,
            };
        }
    }

    public async Task SignOutAsync()
    {
        if (pca == null)
        {
            InitializeClient();
        }

        var accounts = await pca.GetAccountsAsync();

        try
        {
            foreach (var account in accounts)
            {
                await pca.RemoveAsync(account);
            }

            App.CurrentUser = null;
        }
        catch (Exception ex)
        {
            // TODO: handle
            Debug.WriteLine($"MSAL Silent Error: {ex.Message}");
        }
    }

    public async Task<SignInResult> SignInSilentAsync(bool AllowInteractive = true)
    {
        if (pca == null)
        {
            InitializeClient();
        }

        var accounts = await pca.GetAccountsAsync();
        AuthenticationResult result;

        try
        {
            result = await pca
                .AcquireTokenSilent(AuthConstants.Scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            if (AllowInteractive)
            {
                return await SignInInteractive();
            }

            return new SignInResult
            {
                Succeeded = false,
            };
        }

        if (result != null && !string.IsNullOrWhiteSpace(result.AccessToken))
        {
            await SetAuthenticated(result);

            return new SignInResult
            {
                Succeeded = true,
            };
        }

        return new SignInResult
        {
            Succeeded = false,
        };
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrWhiteSpace(accessToken) && !IsTokenExpired())
        {
            return accessToken;
        }

        var accounts = await pca.GetAccountsAsync();

        try
        {
            var result = await pca
                .AcquireTokenSilent(AuthConstants.Scopes, accounts.FirstOrDefault())
                .ExecuteAsync();

            await SetAuthenticated(result);

            return result.AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            // TODO: retry policies
            return string.Empty;
        }
    }

    public string GetUserEmail()
    {
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            throw new Exception("User is not authenticated");
        }

        return userEmail;
    }

    private void InitializeClient()
    {
        var pcaBuilder = PublicClientApplicationBuilder
            .Create(AuthConstants.ClientId)
            .WithAuthority(AuthConstants.AuthoritySignIn)
#if IOS
            .WithIosKeychainSecurityGroup(AuthConstants.IosKeychainSecurityGroup)
#elif ANDROID
            .WithParentActivityOrWindow(() => Platform.CurrentActivity)
#endif
            .WithRedirectUri(AuthConstants.RedirectUri);

        pca = pcaBuilder.Build();
    }

    private async Task<SignInResult> SignInInteractive()
    {
        if (pca == null)
        {
            InitializeClient();
        }

        try
        {
            var result = await pca
                .AcquireTokenInteractive(AuthConstants.Scopes)
                .ExecuteAsync();

            if (result != null && !string.IsNullOrWhiteSpace(result.AccessToken))
            {
                await SetAuthenticated(result);

                return new SignInResult
                {
                    Succeeded = true,
                };
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return new SignInResult
        {
            Succeeded = false,
        };
    }

    private async Task SetAuthenticated(AuthenticationResult result)
    {
        accessToken = result.AccessToken;
        expiresOn = result.ExpiresOn;

        var emailClaim = result.ClaimsPrincipal.Claims.FirstOrDefault(c => c.Type == "email");
        var context = GetUserContext(result);

        if (emailClaim != null)
        {
            userEmail = emailClaim.Value;
            var user = await userManager.GetUserByEmailAsync(context.EmailAddress, context);

            App.CurrentUser = user;
        }

        UserState.UserContext = context;
    }

    private bool IsTokenExpired()
    {
        var bufferTime = TimeSpan.FromMinutes(5);
        return DateTimeOffset.UtcNow > expiresOn - bufferTime;
    }

    private static UserContext GetUserContext(AuthenticationResult ar)
    {
        var newContext = new UserContext
        {
            IsLoggedOn = false,
        };

        var user = ParseIdToken(ar.IdToken);

        newContext.AccessToken = ar.AccessToken;

        if (user != null)
        {
            newContext.EmailAddress = user["email"]?.ToString() ?? user["emailAddress"]?.ToString() ?? string.Empty;
        }

        newContext.IsLoggedOn = true;

        return newContext;
    }

    private static JObject ParseIdToken(string idToken)
    {
        // Get the piece with actual user info
        idToken = idToken.Split('.')[1];
        idToken = Base64UrlDecode(idToken);
        return JObject.Parse(idToken);
    }

    private static string Base64UrlDecode(string s)
    {
        s = s.Replace('-', '+').Replace('_', '/');
        s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
        var byteArray = Convert.FromBase64String(s);
        var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
        return decoded;
    }
}