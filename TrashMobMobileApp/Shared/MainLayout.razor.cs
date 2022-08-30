using Microsoft.AspNetCore.Components;
using TrashMobMobileApp.Authentication;
using TrashMobMobileApp.Data;

namespace TrashMobMobileApp.Shared
{
    public partial class MainLayout
    {
        [Inject]
        public IB2CAuthenticationService AuthenticationService { get; set; }

        [Inject]
        public IUserManager UserManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await AuthenticationService.SignOutAsync(); //sign out on initialize to kick off login
            _ = await AuthenticationService.SignInAsync(UserManager);
        }
    }
}
