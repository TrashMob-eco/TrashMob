namespace TrashMobMobileApp.Authentication;

using System.Diagnostics;
using System.Text;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

public class AuthService : IAuthService
{
    private IPublicClientApplication _pca;

    private string _accessToken;

    private DateTimeOffset _expiresOn;
    
    private string _userEmail;

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

        _pca = pcaBuilder.Build();
    }

    public async Task<SignInResult> SignInAsync()
    {
        if (_pca == null)
        {
            InitializeClient();
        }

        var accounts = await _pca.GetAccountsAsync();
        AuthenticationResult result = null;

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
                Succeeded = false
            };
        }
    }

    public async Task<SignInResult> SignInSilentAsync(bool AllowInteractive = true)
    {
        if (_pca == null)
        {
            InitializeClient();
        }

        var accounts = await _pca.GetAccountsAsync();
        AuthenticationResult result = null;

        try
        {
            result = await _pca
                    .AcquireTokenSilent(AuthConstants.Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            if (AllowInteractive)
            {
                return await SignInInteractive();
            }
            else
            {
                return new SignInResult
                {
                    Succeeded = false
                };
            }
        }

        if (result != null && !string.IsNullOrWhiteSpace(result.AccessToken))
        {
            SetAuthenticated(result);

            return new SignInResult
            {
                Succeeded = true
            };
        }

        return new SignInResult
        {
            Succeeded = false
        };
    }

    private async Task<SignInResult> SignInInteractive()
    {
        if (_pca == null)
        {
            InitializeClient();
        }

        AuthenticationResult result = await _pca
                .AcquireTokenInteractive(AuthConstants.Scopes)
                .ExecuteAsync();

        if (result != null && !string.IsNullOrWhiteSpace(result.AccessToken))
        {
            SetAuthenticated(result);

            return new SignInResult
            {
                Succeeded = true
            };
        }

        return new SignInResult
        {
            Succeeded = false
        };
    }

    private void SetAuthenticated(AuthenticationResult result)
    {
        _accessToken = result.AccessToken;
        _expiresOn = result.ExpiresOn;

        var emailClaim = result.ClaimsPrincipal.Claims.FirstOrDefault(c => c.Type == "email");

        if (emailClaim != null)
        {
            _userEmail = emailClaim.Value;
        }

        UserState.UserContext = GetUserContext(result);
    }

    private bool IsTokenExpired()
    {
        var bufferTime = TimeSpan.FromMinutes(5);
        return DateTimeOffset.UtcNow > _expiresOn - bufferTime;
    }
    
    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrWhiteSpace(_accessToken) && !IsTokenExpired())
        {
            return _accessToken;
        }
        
        var accounts = await _pca.GetAccountsAsync();
        
        try
        {
            var result = await _pca
                    .AcquireTokenSilent(AuthConstants.Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
            
            SetAuthenticated(result);

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
        if (string.IsNullOrWhiteSpace(_userEmail))
        {
            throw new Exception("User is not authenticated");
        }

        return _userEmail;
    }

    private UserContext GetUserContext(AuthenticationResult ar)
    {
        var newContext = new UserContext
        {
            IsLoggedOn = false
        };

        JObject user = ParseIdToken(ar.IdToken);

        newContext.AccessToken = ar.AccessToken;
        newContext.EmailAddress = user["email"]?.ToString() ?? user["emailAddress"]?.ToString();

        newContext.IsLoggedOn = true;

        return newContext;
    }

    private JObject ParseIdToken(string idToken)
    {
        // Get the piece with actual user info
        idToken = idToken.Split('.')[1];
        idToken = Base64UrlDecode(idToken);
        return JObject.Parse(idToken);
    }

    private string Base64UrlDecode(string s)
    {
        s = s.Replace('-', '+').Replace('_', '/');
        s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
        var byteArray = Convert.FromBase64String(s);
        var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
        return decoded;
    }
}
