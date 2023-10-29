namespace TrashMobMobileApp.Authentication;

public interface IAuthService
{
    Task<SignInResult> SignInAsync();

    Task<SignInResult> SignInSilentAsync(bool AllowInteractive = true);

    // TODO: Add other methods
}
