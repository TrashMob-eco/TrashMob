namespace TrashMobMobile.Features.LogOn
{
    using Microsoft.Identity.Client;
    using System.Threading.Tasks;
    using TrashMobMobile.Services;

    public interface IB2CAuthenticationService
    {
        Task<UserContext> EditProfileAsync();
        Task<UserContext> ResetPasswordAsync(IUserManager userManager);
        Task<UserContext> SignInAsync(IUserManager userManager);
        Task<UserContext> SignOutAsync();
        UserContext UpdateUserInfo(AuthenticationResult ar);
    }
}