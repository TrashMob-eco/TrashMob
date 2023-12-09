namespace TrashMobMobile.Authentication;

public static class AuthConstants
{
    private const string SignInPolicy = "B2C_1A_TM_SIGNUP_SIGNIN";
    private const string ResetPasswordPolicyId = "";
    private const string EditProfilePolicyId = "B2C_1A_TM_PROFILEEDIT";
    private const string DeleteProfilePolicyId = "B2C_1A_TM_DEREGISTER";

#if DEBUG
    public const string ClientId = "31cb1c9a-eaa6-4fd0-b59f-0bd0099845ee";
    private const string TenantName = "TrashmobDev";
    private const string TenantId = $"{TenantName}.onmicrosoft.com";
    public const string ApiBaseUri = "https://as-tm-dev-westus2.azurewebsites.net/api/";
#else
    public const string ClientId = "193638ed-30a1-4e29-ba95-fc39f0c0f242";
    private const string TenantName = "Trashmob";
    private const string TenantId = $"{TenantName}.onmicrosoft.com";
    public const string ApiBaseUri = "https://www.trashmob.eco/api/";
#endif

    public static readonly string[] Scopes = new string[]
    {
        $"https://{TenantId}/api/TrashMob.Writes",
        $"https://{TenantId}/api/TrashMob.Read",
        "email",
        "openid",
        "offline_access"
    };

    private const string AuthorityBase = $"https://{TenantName}.b2clogin.com/tfp/{TenantId}/";
    public const string AuthoritySignIn = $"{AuthorityBase}{SignInPolicy}";

    public const string IosKeychainSecurityGroup = "com.microsoft.adalcache";
    public const string RedirectUri = "eco.trashmob.trashmobmobile://auth";
    
    public const string AUTHENTICATED_CLIENT = "AuthenticatedClient";
}
