using System.Net.Http.Headers;

namespace TrashMobMobile.Authentication;

public class AuthHandler : DelegatingHandler
{
    private readonly IAuthService _authService;
    
    public AuthHandler(IAuthService authService)
    {
        _authService = authService;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _authService.GetAccessTokenAsync();
        
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        else
        {
            // TODO: handle retries, reauth etc.
        }
        
        return await base.SendAsync(request, cancellationToken);
    }
}