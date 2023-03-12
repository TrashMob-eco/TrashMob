namespace TrashMobMobileApp.Authentication
{
    using Microsoft.Identity.Client;

    public class B2CConstants
    {
        public string Tenant { get; set; }

        public string AzureADB2CHostname { get; set; }

        public string ClientID { get; set; }

        public string PolicySignUpSignIn { get; set; }

        public string PolicyEditProfile { get; set; }

        public string PolicyResetPassword { get; set; }

        public string AndroidRedirectUri { get; set; }

        public string IOSRedirectUri { get; set; }

        public string Scopes { get; set; }

        public string AuthoritySignInSignUp { get; set; }

        public string IOSKeyChainGroup { get; set; }

        public IPublicClientApplication PublicClientApp { get; set; }

        public string[] ApiScopesArray => Scopes.Split(",");
    }
}