namespace TrashMobMobileApp.Authentication
{
    using Microsoft.Identity.Client;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Data;

    public interface IB2CAuthenticationService
    {
        Task<UserContext> EditProfileAsync();
        Task<UserContext> ResetPasswordAsync(IUserManager userManager);
        Task<UserContext> SignInAsync(IUserManager userManager);
        Task<UserContext> SignOutAsync();
        UserContext UpdateUserInfo(AuthenticationResult ar);
    }
}