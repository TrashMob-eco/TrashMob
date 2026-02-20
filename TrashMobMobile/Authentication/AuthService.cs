namespace TrashMobMobile.Authentication;

using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using TrashMobMobile.Services;

public class AuthService : IAuthService
{
    private readonly IUserManager userManager;

    private string accessToken = string.Empty;

    private DateTimeOffset expiresOn;
    private IPublicClientApplication pca = null!;

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

        _ = await pca!.GetAccountsAsync();

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

        if (pca == null)
        {
            throw new Exception("Unable to initialize authentication client");
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

        var accounts = await pca!.GetAccountsAsync();
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

#if IOS
        // On simulator, MSAL silent acquisition won't work (no keychain).
        // Return empty to trigger re-authentication when the token expires.
        if (DeviceInfo.DeviceType == DeviceType.Virtual)
        {
            return string.Empty;
        }
#endif

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

    public async Task<SignInResult> SignUpInteractiveAsync()
    {
        return await SignInInteractive();
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
            .WithAuthority(AuthConstants.Authority)
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
#if IOS
        // On iOS simulator, MSAL cannot save tokens to keychain without entitlements
        // (which require a provisioning profile not available for simulator builds).
        // Use a manual OAuth 2.0 + PKCE flow instead.
        if (DeviceInfo.DeviceType == DeviceType.Virtual)
        {
            return await SignInInteractiveManual();
        }
#endif

        if (pca == null)
        {
            InitializeClient();
        }

        try
        {
            var result = await pca!
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

    /// <summary>
    /// Manual OAuth 2.0 Authorization Code + PKCE flow for iOS simulator,
    /// bypassing MSAL's keychain-dependent token cache.
    /// </summary>
    private async Task<SignInResult> SignInInteractiveManual()
    {
        try
        {
            // Generate PKCE values
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            var state = Guid.NewGuid().ToString("N");

            var scopes = string.Join(" ", AuthConstants.Scopes);
            var authorizeUrl = $"{AuthConstants.Authority}{AuthConstants.TenantDomain}/oauth2/v2.0/authorize" +
                $"?client_id={Uri.EscapeDataString(AuthConstants.ClientId)}" +
                $"&response_type=code" +
                $"&redirect_uri={Uri.EscapeDataString(AuthConstants.RedirectUri)}" +
                $"&scope={Uri.EscapeDataString(scopes)}" +
                $"&state={state}" +
                $"&code_challenge={codeChallenge}" +
                $"&code_challenge_method=S256";

            var callbackUri = new Uri(AuthConstants.RedirectUri);
            var authResult = await WebAuthenticator.AuthenticateAsync(
                new Uri(authorizeUrl),
                callbackUri);

            var code = authResult.Properties.TryGetValue("code", out var authCode) ? authCode : null;
            if (string.IsNullOrWhiteSpace(code))
            {
                Debug.WriteLine("Manual OAuth: No authorization code in callback");
                return new SignInResult { Succeeded = false };
            }

            // Exchange authorization code for tokens
            using var httpClient = new HttpClient();
            var tokenEndpoint = $"{AuthConstants.Authority}{AuthConstants.TenantDomain}/oauth2/v2.0/token";
            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = AuthConstants.ClientId,
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = AuthConstants.RedirectUri,
                ["code_verifier"] = codeVerifier,
                ["scope"] = scopes,
            });

            var tokenResponse = await httpClient.PostAsync(tokenEndpoint, tokenRequest);
            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Manual OAuth: Token exchange failed: {tokenJson}");
                return new SignInResult { Succeeded = false };
            }

            var tokenData = JObject.Parse(tokenJson);
            var newAccessToken = tokenData["access_token"]?.ToString();
            var idToken = tokenData["id_token"]?.ToString();
            var expiresIn = tokenData["expires_in"]?.ToObject<int>() ?? 3600;

            if (string.IsNullOrWhiteSpace(newAccessToken))
            {
                Debug.WriteLine("Manual OAuth: No access token in response");
                return new SignInResult { Succeeded = false };
            }

            // Store tokens in memory
            accessToken = newAccessToken;
            expiresOn = DateTimeOffset.UtcNow.AddSeconds(expiresIn);

            // Parse ID token for user info
            var userClaims = !string.IsNullOrWhiteSpace(idToken) ? ParseIdToken(idToken) : null;
            var email = userClaims?["email"]?.ToString()
                ?? userClaims?["emailAddress"]?.ToString()
                ?? string.Empty;

            var context = new UserContext
            {
                AccessToken = newAccessToken,
                EmailAddress = email,
                IsLoggedOn = true,
            };

            UserState.UserContext = context;

            if (!string.IsNullOrWhiteSpace(email))
            {
                userEmail = email;
                var user = await userManager.GetUserByEmailAsync(email);
                App.CurrentUser = user;
            }

            return new SignInResult { Succeeded = true };
        }
        catch (TaskCanceledException)
        {
            Debug.WriteLine("Manual OAuth: User cancelled authentication");
            return new SignInResult { Succeeded = false };
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Manual OAuth error: {e}");
            return new SignInResult { Succeeded = false };
        }
    }

    private static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        var hash = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        return Convert.ToBase64String(hash)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private async Task SetAuthenticated(AuthenticationResult result)
    {
        accessToken = result.AccessToken;
        expiresOn = result.ExpiresOn;

        var emailClaim = result.ClaimsPrincipal.Claims.FirstOrDefault(c => c.Type == "email");
        var context = GetUserContext(result);

        // Set user context first so AuthHandler can use the token for the API call below
        UserState.UserContext = context;

        if (emailClaim != null)
        {
            userEmail = emailClaim.Value;
            var user = await userManager.GetUserByEmailAsync(context.EmailAddress);

            App.CurrentUser = user;
        }
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