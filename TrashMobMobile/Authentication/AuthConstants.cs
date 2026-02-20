namespace TrashMobMobile.Authentication;

public static class AuthConstants
{
#if USETEST
    public const string ClientId = "33bfdd2c-80a4-4e6e-b211-337b0467226d";
    private const string TenantName = "trashmobecodev";
    internal const string TenantDomain = "TrashMobEcoDev.onmicrosoft.com";
    public const string ApiBaseUri = "https://dev.trashmob.eco/api/";
#else
    public const string ClientId = "33bfdd2c-80a4-4e6e-b211-337b0467226d"; // TODO: Register prod mobile app in Entra
    private const string TenantName = "trashmobecodev"; // TODO: Update to prod tenant
    internal const string TenantDomain = "TrashMobEcoDev.onmicrosoft.com"; // TODO: Update to prod tenant
    public const string ApiBaseUri = "https://www.trashmob.eco/api/";
#endif

    public static readonly string[] Scopes =
    [
        $"https://{TenantDomain}/api/TrashMob.Writes",
        $"https://{TenantDomain}/api/TrashMob.Read",
        "email",
        "openid",
        "offline_access",
    ];

    public const string Authority = $"https://{TenantName}.ciamlogin.com/";

    public const string IosKeychainSecurityGroup = "com.microsoft.adalcache";
    public const string RedirectUri = "eco.trashmob.trashmobmobile://auth";

    public const string AuthenticatedClient = "AuthenticatedClient";
}
