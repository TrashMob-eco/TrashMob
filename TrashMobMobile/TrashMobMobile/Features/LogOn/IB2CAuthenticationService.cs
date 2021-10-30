using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace TrashMobMobile.Features.LogOn
{
    public interface IB2CAuthenticationService
    {
        Task<UserContext> EditProfileAsync();
        Task<UserContext> ResetPasswordAsync();
        Task<UserContext> SignInAsync();
        Task<UserContext> SignOutAsync();
        UserContext UpdateUserInfo(AuthenticationResult ar);
    }
}