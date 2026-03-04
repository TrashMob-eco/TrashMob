namespace TrashMobMobile.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using TrashMob.Models;
    using TrashMobMobile.Services;

    public partial class EditDependentViewModel(
        IDependentRestService dependentRestService,
        IUserManager userManager,
        INotificationService notificationService)
        : BaseViewModel(notificationService)
    {
        private Guid dependentId;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string pageTitle = "Add Dependent";

        [ObservableProperty]
        private string firstName = string.Empty;

        [ObservableProperty]
        private string lastName = string.Empty;

        [ObservableProperty]
        private DateTime dateOfBirth = DateTime.Today.AddYears(-8);

        [ObservableProperty]
        private string relationship = "Parent";

        [ObservableProperty]
        private string medicalNotes = string.Empty;

        [ObservableProperty]
        private string emergencyContactPhone = string.Empty;

        [ObservableProperty]
        private bool isFormValid;

        public List<string> Relationships { get; } =
        [
            "Parent",
            "Legal Guardian",
            "Grandparent",
            "Authorized Supervisor",
            "Other"
        ];

        public async Task Init(Guid id)
        {
            if (id == Guid.Empty) return;

            dependentId = id;
            IsEditing = true;
            PageTitle = "Edit Dependent";

            await ExecuteAsync(async () =>
            {
                var userId = userManager.CurrentUser.Id;
                var dependents = await dependentRestService.GetDependentsAsync(userId);
                var dependent = dependents.FirstOrDefault(d => d.Id == id);

                if (dependent == null)
                {
                    await Shell.Current.GoToAsync("..");
                    return;
                }

                FirstName = dependent.FirstName ?? string.Empty;
                LastName = dependent.LastName ?? string.Empty;
                DateOfBirth = dependent.DateOfBirth.ToDateTime(TimeOnly.MinValue);
                Relationship = dependent.Relationship ?? "Parent";
                MedicalNotes = dependent.MedicalNotes ?? string.Empty;
                EmergencyContactPhone = dependent.EmergencyContactPhone ?? string.Empty;
            }, "Failed to load dependent details.");
        }

        public void ValidateForm()
        {
            IsFormValid = !string.IsNullOrWhiteSpace(FirstName)
                          && !string.IsNullOrWhiteSpace(LastName)
                          && DateOfBirth < DateTime.Today;
        }

        [RelayCommand]
        private async Task Save()
        {
            ValidateForm();
            if (!IsFormValid) return;

            await ExecuteAsync(async () =>
            {
                var userId = userManager.CurrentUser.Id;

                var dependent = new Dependent
                {
                    Id = IsEditing ? dependentId : Guid.Empty,
                    ParentUserId = userId,
                    FirstName = FirstName.Trim(),
                    LastName = LastName.Trim(),
                    DateOfBirth = DateOnly.FromDateTime(DateOfBirth),
                    Relationship = Relationship,
                    MedicalNotes = string.IsNullOrWhiteSpace(MedicalNotes) ? null : MedicalNotes.Trim(),
                    EmergencyContactPhone = string.IsNullOrWhiteSpace(EmergencyContactPhone) ? null : EmergencyContactPhone.Trim(),
                    IsActive = true,
                };

                if (IsEditing)
                {
                    await dependentRestService.UpdateDependentAsync(userId, dependent);
                    await NotificationService.Notify("Dependent updated.");
                }
                else
                {
                    await dependentRestService.AddDependentAsync(userId, dependent);
                    await NotificationService.Notify("Dependent added.");
                }

                await Shell.Current.GoToAsync("..");
            }, "Failed to save dependent. Please try again.");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
