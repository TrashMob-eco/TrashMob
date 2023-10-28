using Microsoft.Identity.Client;

namespace TrashMobMobileApp.Authentication;

public class AuthService : IAuthService
{
    private readonly IPublicClientApplication _pca;

    public AuthService()
    {
        var pcaBuilder = PublicClientApplicationBuilder
            .Create(AuthConstants.ClientId)
            .WithAuthority(AuthConstants.AuthoritySignIn)
            .WithIosKeychainSecurityGroup(AuthConstants.IosKeychainSecurityGroup)
            .WithRedirectUri(AuthConstants.RedirectUri);
        
        _pca = pcaBuilder.Build();
    }
    
    public Task SignInAsync()
    {
        throw new NotImplementedException();
    }
}