namespace TrashMobMobileApp.Authentication
{
    using Microsoft.Identity.Client;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Data;

    public interface IB2CAuthenticationService
    {
        Task EditProfileAsync();
        Task ResetPasswordAsync(IUserManager userManager);
        Task SignInAsync(IUserManager userManager);
        Task SignOutAsync();
        UserContext UpdateUserInfo(AuthenticationResult ar);
    }
}