namespace TrashMobMobile.ViewModels
{
    using System.Collections.ObjectModel;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using TrashMob.Models;
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

        public ObservableCollection<Dependent> Dependents { get; } = [];

        public async Task Init()
        {
            await ExecuteAsync(async () =>
            {
                var userId = userManager.CurrentUser.Id;
                var dependents = await dependentRestService.GetDependentsAsync(userId);
                Dependents.Clear();
                foreach (var d in dependents.OrderBy(x => x.FirstName))
                {
                    Dependents.Add(d);
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
        private async Task EditDependent(Dependent dependent)
        {
            if (dependent == null) return;
            await Shell.Current.GoToAsync($"{nameof(Pages.EditDependentPage)}?DependentId={dependent.Id}");
        }

        [RelayCommand]
        private async Task DeleteDependent(Dependent dependent)
        {
            if (dependent == null) return;

            var confirmed = await Shell.Current.DisplayAlert(
                "Remove Dependent",
                $"Are you sure you want to remove {dependent.FirstName} {dependent.LastName}?",
                "Remove", "Cancel");

            if (!confirmed) return;

            await ExecuteAsync(async () =>
            {
                var userId = userManager.CurrentUser.Id;
                await dependentRestService.DeleteDependentAsync(userId, dependent.Id);
                Dependents.Remove(dependent);
                AreDependentsFound = Dependents.Count > 0;
                AreNoDependentsFound = !AreDependentsFound;
                await NotificationService.Notify("Dependent removed.");
            }, "Failed to remove dependent. Please try again.");
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await Init();
        }
    }
}
