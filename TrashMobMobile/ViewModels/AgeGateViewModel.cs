namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Services;

public partial class AgeGateViewModel(INotificationService notificationService) : BaseViewModel(notificationService)
{
    [ObservableProperty]
    private DateTime dateOfBirth = DateTime.Today.AddYears(-18);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBlocked))]
    private bool isBlocked;

    public bool IsNotBlocked => !IsBlocked;

    [ObservableProperty]
    private string blockMessage = string.Empty;

    [ObservableProperty]
    private bool isAgeVerified;

    public DateTime MaxDate => DateTime.Today;
    public DateTime MinDate => DateTime.Today.AddYears(-120);

    [RelayCommand]
    private void Continue()
    {
        var age = CalculateAge(DateOfBirth);

        if (age < 13)
        {
            IsBlocked = true;
            BlockMessage = "You must be 13 or older to join TrashMob. Thank you for your interest in keeping our communities clean!";
            return;
        }

        IsAgeVerified = true;
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    private static int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
