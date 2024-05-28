namespace TrashMobMobile.Authentication;

using System.Net.Http.Headers;

public class AuthHandler : DelegatingHandler
{
    private readonly IAuthService authService;

    public AuthHandler(IAuthService authService)
    {
        this.authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = await authService.GetAccessTokenAsync();

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        // TODO: handle retries, reauth etc.
        return await base.SendAsync(request, cancellationToken);
    }
}