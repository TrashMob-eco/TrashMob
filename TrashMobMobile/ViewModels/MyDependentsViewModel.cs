namespace TrashMobMobile.ViewModels
{
    using System.Collections.ObjectModel;
    using CommunityToolkit.Maui.Extensions;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using TrashMob.Models;
    using TrashMobMobile.Controls;
    using TrashMobMobile.Services;

    public partial class MyDependentsViewModel(
        IDependentRestService dependentRestService,
        IUserManager userManager,
        INotificationService notificationService)
        : BaseViewModel(notificationService)
    {
        [ObservableProperty]
        private bool areDependentsFound;

        [ObservableProperty]
        private bool areNoDependentsFound = true;

        public ObservableCollection<DependentWithInvitation> Dependents { get; } = [];

        public async Task Init()
        {
            await ExecuteAsync(async () =>
            {
                var userId = userManager.CurrentUser.Id;
                var dependents = await dependentRestService.GetDependentsAsync(userId);
                Dependents.Clear();

                foreach (var d in dependents.OrderBy(x => x.FirstName))
                {
                    DependentInvitation? activeInvitation = null;

                    var wrapper = new DependentWithInvitation(d);
                    if (wrapper.Age >= 13)
                    {
                        try
                        {
                            var invitations = await dependentRestService.GetDependentInvitationsAsync(userId, d.Id);
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

                    Dependents.Add(new DependentWithInvitation(d, activeInvitation));
                }

                AreDependentsFound = Dependents.Count > 0;
                AreNoDependentsFound = !AreDependentsFound;
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
        private async Task InviteDependent(DependentWithInvitation item)
        {
            if (item == null) return;

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
        private async Task Refresh()
        {
            await Init();
        }
    }
}
