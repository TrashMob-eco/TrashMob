namespace TrashMobMobile.ViewModels
{
    using System.Collections.ObjectModel;
    using CommunityToolkit.Maui.Extensions;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMobMobile.Controls;
    using TrashMobMobile.Services;

    public partial class MyDependentsViewModel(
        IDependentRestService dependentRestService,
        IPrivoConsentRestService privoConsentRestService,
        IUserManager userManager,
        INotificationService notificationService)
        : BaseViewModel(notificationService)
    {
        [ObservableProperty]
        private bool areDependentsFound;

        [ObservableProperty]
        private bool areNoDependentsFound = true;

        [ObservableProperty]
        private bool hasEligibleDependents;

        [ObservableProperty]
        private string eligibleDependentsMessage = string.Empty;

        [ObservableProperty]
        private bool isIdentityVerified;

        public ObservableCollection<DependentWithInvitation> Dependents { get; } = [];

        public async Task Init()
        {
            await ExecuteAsync(async () =>
            {
                var userId = userManager.CurrentUser.Id;
                var dtos = await dependentRestService.GetDependentDtosAsync(userId);
                Dependents.Clear();

                foreach (var dto in dtos.OrderBy(x => x.FirstName))
                {
                    var entity = dto.ToEntity();
                    DependentInvitation? activeInvitation = null;

                    var tempWrapper = new DependentWithInvitation(entity);
                    if (tempWrapper.Age >= 13)
                    {
                        try
                        {
                            var invitations = await dependentRestService.GetDependentInvitationsAsync(userId, entity.Id);
                            activeInvitation = invitations
                                .OrderByDescending(i => i.DateInvited)
                                .FirstOrDefault(i =>
                                    i.InvitationStatusId == (int)InvitationStatusEnum.Sent ||
                                    i.InvitationStatusId == (int)InvitationStatusEnum.Accepted);
                        }
                        catch
                        {
                            // Non-critical — just show the dependent without invitation info
                        }
                    }

                    Dependents.Add(new DependentWithInvitation(entity, activeInvitation, dto.PrivoConsentStatus));
                }

                AreDependentsFound = Dependents.Count > 0;
                AreNoDependentsFound = !AreDependentsFound;
                IsIdentityVerified = userManager.CurrentUser.IsIdentityVerified;

                var needsConsentCount = Dependents.Count(d => d.NeedsConsent);
                var eligibleCount = Dependents.Count(d => d.IsEligibleForInvite);
                HasEligibleDependents = eligibleCount > 0 || needsConsentCount > 0;

                if (needsConsentCount > 0 && !IsIdentityVerified)
                {
                    EligibleDependentsMessage = "Verify your identity first to manage consent and send invitations for your 13+ dependents.";
                }
                else if (needsConsentCount > 0)
                {
                    EligibleDependentsMessage = $"{needsConsentCount} dependent(s) need parental consent before they can be invited. Tap Consent to start.";
                }
                else if (eligibleCount > 0)
                {
                    EligibleDependentsMessage = $"{eligibleCount} dependent(s) are ready to be invited. Tap Invite to send them an invitation!";
                }
                else
                {
                    EligibleDependentsMessage = string.Empty;
                    HasEligibleDependents = false;
                }
            }, "Failed to load dependents. Please try again.");
        }

        [RelayCommand]
        private async Task AddDependent()
        {
            await Shell.Current.GoToAsync(nameof(Pages.EditDependentPage));
        }

        [RelayCommand]
        private async Task EditDependent(DependentWithInvitation item)
        {
            if (item == null) return;
            await Shell.Current.GoToAsync($"{nameof(Pages.EditDependentPage)}?DependentId={item.Dependent.Id}");
        }

        [RelayCommand]
        private async Task DeleteDependent(DependentWithInvitation item)
        {
            if (item == null) return;

            var confirmed = await Shell.Current.DisplayAlertAsync(
                "Remove Dependent",
                $"Are you sure you want to remove {item.Dependent.FirstName} {item.Dependent.LastName}?",
                "Remove", "Cancel");

            if (!confirmed) return;

            await ExecuteAsync(async () =>
            {
                var userId = userManager.CurrentUser.Id;
                await dependentRestService.DeleteDependentAsync(userId, item.Dependent.Id);
                Dependents.Remove(item);
                AreDependentsFound = Dependents.Count > 0;
                AreNoDependentsFound = !AreDependentsFound;
                await NotificationService.Notify("Dependent removed.");
            }, "Failed to remove dependent. Please try again.");
        }

        [RelayCommand]
        private async Task VerifyIdentity()
        {
            await Shell.Current.GoToAsync(nameof(Pages.VerifyIdentityPage));
        }

        [RelayCommand]
        private async Task InviteDependent(DependentWithInvitation item)
        {
            if (item == null) return;

            if (!userManager.CurrentUser.IsIdentityVerified)
            {
                var goToVerify = await Shell.Current.DisplayAlertAsync(
                    "Identity Verification Required",
                    "You must verify your identity before inviting dependents aged 13-17. Would you like to verify now?",
                    "Verify Now", "Cancel");

                if (goToVerify)
                {
                    await Shell.Current.GoToAsync(nameof(Pages.VerifyIdentityPage));
                }

                return;
            }

            var popup = new InviteEmailPopup(item.Dependent.FirstName);
            var popupResult = await Shell.Current.CurrentPage.ShowPopupAsync<string>(popup);
            var email = popupResult?.Result;

            if (string.IsNullOrWhiteSpace(email)) return;

            await ExecuteAsync(async () =>
            {
                var userId = userManager.CurrentUser.Id;
                await dependentRestService.CreateDependentInvitationAsync(userId, item.Dependent.Id, email);
                await NotificationService.Notify($"Invitation sent to {email}.");
                await Init();
            }, "Failed to send invitation. Please try again.");
        }

        [RelayCommand]
        private async Task CancelInvitation(DependentWithInvitation item)
        {
            if (item?.ActiveInvitation == null) return;

            var confirmed = await Shell.Current.DisplayAlertAsync(
                "Cancel Invitation",
                $"Cancel the invitation for {item.Dependent.FirstName}?",
                "Cancel Invitation", "Keep");

            if (!confirmed) return;

            await ExecuteAsync(async () =>
            {
                await dependentRestService.CancelDependentInvitationAsync(item.ActiveInvitation.Id);
                await NotificationService.Notify("Invitation canceled.");
                await Init();
            }, "Failed to cancel invitation. Please try again.");
        }

        [RelayCommand]
        private async Task ResendInvitation(DependentWithInvitation item)
        {
            if (item?.ActiveInvitation == null) return;

            await ExecuteAsync(async () =>
            {
                await dependentRestService.ResendDependentInvitationAsync(item.ActiveInvitation.Id);
                await NotificationService.Notify($"Invitation resent to {item.ActiveInvitation.Email}.");
                await Init();
            }, "Failed to resend invitation. Please try again.");
        }

        [RelayCommand]
        private async Task StartConsent(DependentWithInvitation item)
        {
            if (item == null) return;

            if (!userManager.CurrentUser.IsIdentityVerified)
            {
                var goToVerify = await Shell.Current.DisplayAlertAsync(
                    "Identity Verification Required",
                    "You must verify your identity before providing consent for dependents aged 13-17. Would you like to verify now?",
                    "Verify Now", "Cancel");

                if (goToVerify)
                {
                    await Shell.Current.GoToAsync(nameof(Pages.VerifyIdentityPage));
                }

                return;
            }

            await ExecuteAsync(async () =>
            {
                var consent = await privoConsentRestService.InitiateChildConsentAsync(item.Dependent.Id);

                if (!string.IsNullOrEmpty(consent.ConsentUrl))
                {
                    await Browser.Default.OpenAsync(consent.ConsentUrl, BrowserLaunchMode.SystemPreferred);
                }
                else
                {
                    await NotificationService.Notify("Consent request initiated. Check your email for a link from PRIVO.");
                }

                await Init();
            }, "Failed to initiate consent. Please try again.");
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await Init();
        }
    }
}
