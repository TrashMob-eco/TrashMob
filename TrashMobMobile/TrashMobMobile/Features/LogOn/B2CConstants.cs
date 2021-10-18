namespace TrashMobMobile.Features.LogOn
{
    public static class B2CConstants
    {
        // Azure AD B2C Coordinates
        public static string Tenant = "Trashmob.onmicrosoft.com";
        public static string AzureADB2CHostname = "Trashmob.b2clogin.com";
        public static string ClientID = "e8d8517e-095b-4797-bce8-8f6f19d71e3c";
        public static string PolicySignUpSignIn = "B2C_1_signupsignin1";
        public static string PolicyEditProfile = "B2C_1_editprofile1";
        public static string PolicyResetPassword = "B2C_1_passwordreset1";

        public static string[] Scopes = { "https://Trashmob.onmicrosoft.com/mobileapi/TrashMob.Writes", "https://Trashmob.onmicrosoft.com/mobileapi/TrashMob.Reads" };

        public static string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
        public static string AuthoritySignInSignUp = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";
        public static string IOSKeyChainGroup = "com.microsoft.adalcache";
    }
}