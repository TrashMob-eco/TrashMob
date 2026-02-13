namespace TrashMobMobile.Authentication;

public interface IAuthService
{
    Task<SignInResult> SignInAsync();

    Task SignOutAsync();

    Task<SignInResult> SignInSilentAsync(bool AllowInteractive = true);

    Task<string> GetAccessTokenAsync();

    Task<SignInResult> SignUpInteractiveAsync();

    string GetUserEmail();
}