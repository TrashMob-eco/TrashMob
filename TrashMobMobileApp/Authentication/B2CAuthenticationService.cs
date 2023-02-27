namespace TrashMobMobileApp.Authentication
{
    using Microsoft.AppCenter.Analytics;
    using Microsoft.AppCenter.Crashes;
    using Microsoft.Identity.Client;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Data;

    /// <summary>
    ///  For simplicity, we'll have this as a singleton. 
    /// </summary>
    public class B2CAuthenticationService : IB2CAuthenticationService
    {
        private readonly B2CConstants b2CConstants;

        public B2CAuthenticationService(B2CConstants b2CConstants)
        {
#if ANDROID
            // default redirectURI; each platform specific project will have to override it with its own
            var builder = PublicClientApplicationBuilder.Create(b2CConstants.ClientID)
                .WithB2CAuthority(b2CConstants.AuthoritySignInSignUp)
                .WithParentActivityOrWindow(() => Platform.CurrentActivity)
                .WithRedirectUri(b2CConstants.AndroidRedirectUri);
            b2CConstants.PublicClientApp = builder.Build();
#elif IOS
            // default redirectURI; each platform specific project will have to override it with its own
            var builder = PublicClientApplicationBuilder.Create(b2CConstants.ClientID)
                .WithB2CAuthority(b2CConstants.AuthoritySignInSignUp)
                .WithIosKeychainSecurityGroup(b2CConstants.IOSKeyChainGroup)
                .WithRedirectUri(b2CConstants.IOSRedirectUri);
            b2CConstants.PublicClientApp = builder.Build();
#endif
      
            this.b2CConstants = b2CConstants;
        }

        public async Task<UserContext> SignInAsync()
        {
            UserContext newContext;
            try
            {
                // acquire token silent
                newContext = await AcquireTokenSilent();
            }
            catch (MsalUiRequiredException)
            {
                // acquire token interactive
                newContext = await SignInInteractively();
            }

            return newContext;
        }

        public async Task SignInAsync(IUserManager userManager)             
        {
            UserContext localUserContext = null;
            try
            {
                Analytics.TrackEvent("SignInAsync");

                // Copy to a local variable first to make sure the context is populated on next call (threading issue on property setter)            
                localUserContext = await SignInAsync();

                await VerifyAccount(userManager, localUserContext);
                UserState.UserContext = localUserContext;
            }
            catch(Exception ex)
            {
                var properties = new Dictionary<string, string>();

                if (localUserContext != null)
                {
                    properties.Add("emailAddress", localUserContext.EmailAddress);
                    properties.Add("givenName", localUserContext.GivenName);
                    properties.Add("accessToken", localUserContext.AccessToken);
                }

                Crashes.TrackError(ex, properties);
            }
        }

        private static async Task VerifyAccount(IUserManager userManager, UserContext userContext)
        {
            Analytics.TrackEvent("VerifyAccount");
            App.CurrentUser = await userManager.GetUserByEmailAsync(UserState.UserContext.EmailAddress, userContext);
        }

        private async Task<UserContext> AcquireTokenSilent()
        {
            IEnumerable<IAccount> accounts = await b2CConstants.PublicClientApp.GetAccountsAsync();
            AuthenticationResult authResult = await b2CConstants.PublicClientApp.AcquireTokenSilent(b2CConstants.ApiScopesArray, GetAccountByPolicy(accounts, b2CConstants.PolicySignUpSignIn))
               .WithB2CAuthority(b2CConstants.AuthoritySignInSignUp)
               .ExecuteAsync();

            var newContext = UpdateUserInfo(authResult);
            return newContext;
        }

        private async Task<UserContext> SignInInteractively()
        {
            var useEmbeddedWebview = true;
            try
            {
                // Android implementation is based on https://github.com/jamesmontemagno/CurrentActivityPlugin
                // iOS implementation would require to expose the current ViewControler - not currently implemented as it is not required
                // UWP does not require this
                var windowLocatorService = DependencyService.Get<IParentWindowLocatorService>();

                AuthenticationResult authResult;

                if (windowLocatorService == null)
                {
                    authResult = await b2CConstants.PublicClientApp.AcquireTokenInteractive(b2CConstants.ApiScopesArray)
                                        .WithUseEmbeddedWebView(useEmbeddedWebview)
                                        .ExecuteAsync();
                }
                else
                {
                    authResult = await b2CConstants.PublicClientApp.AcquireTokenInteractive(b2CConstants.ApiScopesArray)
                                        .WithParentActivityOrWindow(windowLocatorService?.GetCurrentParentWindow())
                                        .WithUseEmbeddedWebView(useEmbeddedWebview)
                                        .ExecuteAsync();
                }

                var newContext = UpdateUserInfo(authResult);
                return newContext;
            }
            catch
            {
                throw;
            }
        }

        public async Task SignOutAsync()
        {

            IEnumerable<IAccount> accounts = await b2CConstants.PublicClientApp.GetAccountsAsync();
            while (accounts.Any())
            {
                await b2CConstants.PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                accounts = await b2CConstants.PublicClientApp.GetAccountsAsync();
            }

            var signedOutContext = new UserContext
            {
                IsLoggedOn = false
            };

            UserState.UserContext = signedOutContext;
        }

        private IAccount GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
        {
            foreach (var account in accounts)
            {
                string userIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
                if (userIdentifier.EndsWith(policy.ToLower()))
                {
                    return account;
                }
            }

            return null;
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
            newContext.EmailAddress = user["email"]?.ToString();

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
}