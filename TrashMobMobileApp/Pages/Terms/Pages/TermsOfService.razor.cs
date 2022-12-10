using Microsoft.AspNetCore.Components;
using TrashMob.Models;
using TrashMobMobileApp.Data;

namespace TrashMobMobileApp.Pages.Terms.Pages
{
    public partial class TermsOfService
    {
        private bool _isLoading;
        private bool _agreeTermsOfService;
        private bool _agreePrivacyPolicy;
        private DateTimeOffset? _dateTermsSigned;
        private DateTimeOffset? _datePrivacySigned;
        private User _user;

        [Inject]
        public IUserManager UserManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TitleContainer.Title = "Terms and Privacy";
            _isLoading = true;
            _user = await UserManager.GetUserAsync(App.CurrentUser.Id.ToString());
            _isLoading = false;
            _agreeTermsOfService = _user.DateAgreedToTermsOfService.HasValue ? true : false;
            _agreePrivacyPolicy = _user.DateAgreedToPrivacyPolicy.HasValue ? true : false;
        }

        private void OnTermsChange(bool val)
        {
            _agreeTermsOfService = val;
            if (_agreeTermsOfService)
            {
                _dateTermsSigned = DateTimeOffset.UtcNow;
            }
            else
            {
                _dateTermsSigned = null;
            }
        }

        private void OnPrivacyChange(bool val)
        {
            _agreePrivacyPolicy = val;
            if (_agreePrivacyPolicy)
            {
                _datePrivacySigned = DateTimeOffset.UtcNow;
            }
            else
            {
                _datePrivacySigned = null;
            }
        }

        private async Task OnSaveAsync()
        {
            if (_user != null)
            {
                _user.DateAgreedToTermsOfService = _dateTermsSigned;
                _user.DateAgreedToPrivacyPolicy = _datePrivacySigned;

                _isLoading = true;
                await UserManager.UpdateUserAsync(_user);
                _isLoading = false;
                Snackbar.Add("Changes saved!", MudBlazor.Severity.Success);
            }
        }
    }
}
