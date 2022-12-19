using Microsoft.AspNetCore.Components;
using MudBlazor;
using TrashMob.Models;
using TrashMobMobileApp.Data;

namespace TrashMobMobileApp.Pages.Profile.Pages
{
    public partial class UserProfile
    {
        private User _user;
        private bool _isLoading;
        private MudForm? _userForm;
        private bool _success;
        private string[] _errors;

        [Inject]
        public IUserManager UserManager { get; set; } 

        protected override async Task OnInitializedAsync()
        {
            TitleContainer.Title = "Profile";
            await GetUserAsync();
        }

        private async Task GetUserAsync()
        {
            try
            {
                _isLoading = true;
                _user = await UserManager.GetUserAsync(App.CurrentUser.Id.ToString());
                _isLoading = false;
            }
            catch (Exception)
            {
                //log exception somewhere
                //try user service one more time
                _isLoading = true;
                _user = await UserManager.GetUserAsync(App.CurrentUser.Id.ToString());
                _isLoading = false;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private string GetDateString(DateTimeOffset? date)
        {
            if (date.HasValue)
            {
                return date.Value.DateTime.ToLocalTime().ToString();
            }

            return string.Empty;
        }

        private async Task OnSaveUserAsync()
        {
            await _userForm.Validate();
            if (_success)
            {
                _isLoading = true;
                var user = await UserManager.UpdateUserAsync(_user);
                _isLoading = false;
                if (user != null)
                {
                    await GetUserAsync();
                }
                Snackbar.Add("Changes saved!", Severity.Success);
            }
        }
    }
}
