using MudBlazor;
using TrashMobMobileApp.Models;

namespace TrashMobMobileApp.Pages.Profile.Pages
{
    public partial class UserProfile
    {
        private User _user;
        private bool _isLoading;
        private MudForm? _userForm;
        private bool _success;
        private string[] _errors;

        protected override void OnInitialized()
        {
            TitleContainer.Title = "Profile";
            _user = App.CurrentUser;
        }
    }
}
