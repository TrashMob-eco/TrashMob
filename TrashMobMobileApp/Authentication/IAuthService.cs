namespace TrashMobMobileApp.Authentication;

public interface IAuthService
{
    Task<SignInResult> SignInAsync();

    Task<SignInResult> SignInSilentAsync();
    
    // TODO: Add other methods
}