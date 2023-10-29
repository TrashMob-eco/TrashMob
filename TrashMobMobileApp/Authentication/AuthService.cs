using System.Diagnostics;
using Microsoft.Identity.Client;

namespace TrashMobMobileApp.Authentication;

public class AuthService : IAuthService
{
    private IPublicClientApplication _pca;

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
            // TODO: handle set logged in state

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

        AuthenticationResult result = null;

        result = await _pca
                .AcquireTokenInteractive(AuthConstants.Scopes)
                .ExecuteAsync();

        if (result != null && !string.IsNullOrWhiteSpace(result.AccessToken))
        {
            // TODO: handle set logged in state

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
}
