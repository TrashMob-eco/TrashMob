namespace TrashMobMobile.ViewModels
{
    using System;
    using TrashMobMobile.Services;
    using TrashMobMobile.Views;
    using Xamarin.Forms;

    public class TermsAndConditionsViewModel : BaseViewModel
    {
        private bool isTermsOfUseAgreed;
        private bool isPrivacyPolicyAgreed;
        private readonly IUserManager userManager;

        public TermsAndConditionsViewModel(IUserManager userManager)
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(async () => await App.Current.MainPage.Navigation.PopModalAsync());
            PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            this.userManager = userManager;
            IsTermsOfUseAgreed = App.CurrentUser.DateAgreedToTermsOfService != null && App.CurrentUser.DateAgreedToTermsOfService != DateTimeOffset.MinValue;
            IsPrivacyPolicyAgreed = App.CurrentUser.DateAgreedToPrivacyPolicy != null && App.CurrentUser.DateAgreedToPrivacyPolicy != DateTimeOffset.MinValue;
        }

        private bool ValidateSave()
        {
            return isTermsOfUseAgreed && isPrivacyPolicyAgreed;
        }

        public bool IsTermsOfUseAgreed
        {
            get => isTermsOfUseAgreed;
            set => SetProperty(ref isTermsOfUseAgreed, value);
        }

        public bool IsPrivacyPolicyAgreed
        {
            get => isPrivacyPolicyAgreed;
            set => SetProperty(ref isPrivacyPolicyAgreed, value);
        }

        public Command SaveCommand { get; }

        public Command CancelCommand { get; }

        private async void OnSave()
        {
            var user = App.CurrentUser;

            user.TermsOfServiceVersion = Constants.TermsOfServiceVersion;
            user.PrivacyPolicyVersion = Constants.PrivacyPolicyVersion;
            user.DateAgreedToPrivacyPolicy = DateTimeOffset.UtcNow;
            user.DateAgreedToTermsOfService = DateTimeOffset.UtcNow;

            // Todo fix this so that Update User returns the updated user
            await userManager.UpdateUserAsync(user);
            App.CurrentUser = await userManager.GetUserAsync(user.Id);

            // If the user name is not present, redirect to User Profile Page to allow user to fill it in
            if (string.IsNullOrWhiteSpace(App.CurrentUser.UserName) || App.CurrentUser.UserName.Contains("joe"))
            {
                await Shell.Current.GoToAsync($"//{nameof(UserProfilePage)}");
            }
            else
            {
                await Shell.Current.GoToAsync($"//{nameof(MobEventsPage)}");
            }
        }
    }
}
